using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Orders;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Orders;
using Shop.Api.Utils;
using Shop.Commands.Orders;
using Shop.Commands.Payments;
using Shop.Common.Enums;
using Shop.ReadModel.Orders;
using Shop.ReadModel.Orders.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common;
using Xia.Common.Extensions;
using Xia.Common.Utils;

namespace Shop.Api.Controllers
{
    [ApiAuthorizeFilter]
    [EnableCors(origins: "http://app.wftx666.com,http://localhost:51776,http://localhost:8080", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class OrderController:BaseApiController
    {
        //private static readonly TimeSpan DraftOrderWaitTimeout = TimeSpan.FromSeconds(5);
        //private static readonly TimeSpan DraftOrderPollInterval = TimeSpan.FromMilliseconds(750);
        private static readonly TimeSpan PricedOrderWaitTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan PricedOrderPollInterval = TimeSpan.FromMilliseconds(750);

        private ICommandService _commandService;//C端
        private OrderQueryService _orderQueryService;//Q 端

        public OrderController(ICommandService commandService, 
            OrderQueryService orderQueryService)
        {
            _commandService = commandService;
            _orderQueryService = orderQueryService;
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Order/Add")]
        public async Task<BaseApiResponse> Add(AddOrderRequest request)
        {
            request.CheckNotNull(nameof(request));
            request.ExpressAddress.CheckNotNull(nameof(request.ExpressAddress));

            TryInitUserModel();

            var command = new PlaceOrderCommand(
                GuidUtil.NewSequentialId(),
                _user.Id,
                new Commands.Orders.ExpressAddressInfo(
                    request.ExpressAddress.Region,
                    request.ExpressAddress.Address,
                    request.ExpressAddress.Name,
                    request.ExpressAddress.Mobile,
                    request.ExpressAddress.Zip
                    ),
                request.CartGoodses.Select(x => new SpecificationInfo(
                    x.SpecificationId,
                    x.GoodsId,
                    x.StoreId,
                    x.GoodsName,
                    x.GoodsPic,
                    x.SpecificationName,
                    x.Price,
                    x.OriginalPrice,
                    x.Quantity,
                    0.15M
                    )).ToList());
            if (!command.Specifications.Any())
            {
                return new BaseApiResponse { Code = 400, Message = "订单至少包含一个商品" };
            }
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            //等待订单预定成功
            var order = WaitUntilReservationCompleted(command.AggregateRootId).Result;
            if (order == null)
            {
                return new BaseApiResponse { Code = 400, Message = "未知的订单" };
            }

            //预定成功，处理付款信息，要求用户付款
            if (order.Status == (int)OrderStatus.ReservationSuccess)
            {
                return await StartPayment(order.OrderId);
            }
            else
            {
                return new BaseApiResponse { Code = 400, Message = "预定失败" };
            }
            
        }
        

        #region 私有方法
        /// <summary>
        /// 开启订单支付
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private async Task<BaseApiResponse> StartPayment(Guid orderId)
        {
            var order = _orderQueryService.FindOrder(orderId);

            if (order == null)
            {
                return new BaseApiResponse { Code = 400, Message = "没有该预定单" };
            }
            if (order.Status == (int)OrderStatus.PaymentSuccess || order.Status == (int)OrderStatus.Success)
            {
                return new BaseApiResponse { Code = 400, Message = "预定单已经付款或完成" };
            }
            if (order.ReservationExpirationDate.HasValue && order.ReservationExpirationDate < DateTime.Now)
            {
                return new BaseApiResponse { Code = 400, Message = "预订单已经过期，请重新预定" };
            }
            //if (order.IsFreeOfCharge)
            //{
            //    //免支付订单，直接设置成功
            //    return await CompleteRegistrationWithoutPayment(orderId, order.GoodsId);
            //}
            //要通过支付完成预订单
            return await CompleteRegistrationWithThirdPartyProcessorPayment(order);
        }
        /// <summary>轮训订单状态，直到订单的库存预扣操作完成
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private Task<Order> WaitUntilReservationCompleted(Guid orderId)
        {
            return TimerTaskFactory.StartNew<Order>(
                    () => _orderQueryService.FindOrder(orderId),
                    x => x != null && (x.Status == (int)OrderStatus.ReservationSuccess || x.Status == (int)OrderStatus.ReservationFailed),
                    PricedOrderPollInterval,
                    PricedOrderWaitTimeout);
        }

        //完成预定切未付款
        private async Task<BaseApiResponse> CompleteRegistrationWithoutPayment(Guid orderId,Guid goodsId)
        {
            var command= new MarkAsSuccessCommand(orderId,goodsId);
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "创建付款信息失败：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }
        /// <summary>
        /// 通过第三方支付，完成预订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private async Task<BaseApiResponse> CompleteRegistrationWithThirdPartyProcessorPayment(Order order)
        {
            //创建付款信息
            var paymentCommand = CreatePaymentCommand(order);

            var result = await ExecuteCommandAsync(paymentCommand);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "创建付款信息失败：{0}".FormatWith(result.GetErrorMessage()) };
            }
            //返回订单ID，付款ID，要求前端进入付款页
            return new AddOrderResponse
            {
                OrderId = order.OrderId,
                PaymentId=paymentCommand.AggregateRootId
            };
        }


        private CreatePaymentCommand CreatePaymentCommand(Order order)
        {
            var description = "支付订单 " + order.OrderId;
            var paymentCommand = new CreatePaymentCommand
            {
                AggregateRootId = GuidUtil.NewSequentialId(),
                OrderId = order.OrderId,
                Description = description,
                TotalAmount = order.Total,
                Lines = order.Lines.Select(x => new PaymentLine { Id = x.SpecificationId, Description = x.SpecificationName, Amount = x.LineTotal })
            };

            return paymentCommand;
        }

        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.CommandExecuted).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}