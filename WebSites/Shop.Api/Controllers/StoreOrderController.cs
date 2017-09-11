using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Orders;
using Shop.Api.Models.Request.StoreOrders;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Orders;
using Shop.Api.Models.Response.StoreOrders;
using Shop.Api.Utils;
using Shop.Commands.Orders;
using Shop.Commands.Payments;
using Shop.Commands.Stores.StoreOrders;
using Shop.Common.Enums;
using Shop.ReadModel.Orders;
using Shop.ReadModel.Orders.Dtos;
using Shop.ReadModel.StoreOrders;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common;
using Xia.Common.Extensions;
using System.Collections.Generic;

namespace Shop.Api.Controllers
{
    /// <summary>
    /// 商家订单-包裹服务
    /// </summary>
    [ApiAuthorizeFilter]
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class StoreOrderController:BaseApiController
    {

        private ICommandService _commandService;//C端
        private StoreOrderQueryService _storeOrderQueryService;//Q 端

        public StoreOrderController(ICommandService commandService, 
            StoreOrderQueryService storeOrderQueryService)
        {
            _commandService = commandService;
            _storeOrderQueryService = storeOrderQueryService;
        }

        #region 查询
        /// <summary>
        /// 包裹详情
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StoreOrder/Info")]
        public BaseApiResponse Info(InfoRequest request)
        {
            request.CheckNotNull(nameof(request));
            var storeorder = _storeOrderQueryService.FindOrder(request.Id);
            if(storeorder==null)
            {
                return new BaseApiResponse { Code = 400, Message = "未找到该订单" };
            }
            return new InfoResponse
            {
                Id = storeorder.Id,
                StoreId = storeorder.StoreId,
                Number = storeorder.Number,
                Region = storeorder.Region,
                Remark = storeorder.Remark,
                ExpressRegion = storeorder.ExpressRegion,
                ExpressAddress = storeorder.ExpressAddress,
                ExpressName = storeorder.ExpressName,
                ExpressMobile = storeorder.ExpressMobile,
                ExpressZip = storeorder.ExpressZip,
                CreatedOn = storeorder.CreatedOn,
                Total = storeorder.Total,
                Status = storeorder.Status.ToDescription(),
                StoreOrderGoodses = storeorder.StoreOrderGoodses.Select(z => new StoreOrderGoods
                {
                    Id = z.Id,
                    GoodsId = z.GoodsId,
                    SpecificationId = z.SpecificationId,
                    SpecificationName = z.SpecificationName,
                    GoodsName = z.GoodsName,
                    GoodsPic = z.GoodsPic,
                    Quantity = z.Quantity,
                    Price = z.Price,
                    OriginalPrice = z.OriginalPrice,
                    Total = z.Total,
                    StoreTotal = z.StoreTotal
                }).ToList()
            };
        }

        /// <summary>
        /// 物流状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("StoreOrder/ExpressSchedule")]
        public string ExpressSchedule(ExpressScheduleRequest request)
        {
            request.CheckNotNull(nameof(request));

            var expressSchedule = ExpressScheduleUtil.GetSchedule("auto", request.ExpressNumber);

            return expressSchedule;
        }


        /// <summary>
        /// 用户的订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("StoreOrder/UserOrders")]
        public BaseApiResponse UserOrders(UserOrdersRequest request)
        {
            TryInitUserModel();
            int pageRecordCount = 10;
            //获取指定状态的订单 分页数据
            IEnumerable<ReadModel.StoreOrders.Dtos.StoreOrderDetails> storeOrders = null;
            if (request.Status==StoreOrderStatus.All)
            {
                storeOrders = _storeOrderQueryService.UserStoreOrderDetails(_user.Id).OrderByDescending(x => x.CreatedOn).Skip(pageRecordCount * request.Page).Take(pageRecordCount);
            }
            else
            {
                storeOrders = _storeOrderQueryService.UserStoreOrderDetails(_user.Id,request.Status).OrderByDescending(x=>x.CreatedOn).Skip(pageRecordCount*request.Page).Take(pageRecordCount);
            }

            return new UserOrdersResponse
            {
                StoreOrders = storeOrders.Select(x => new StoreOrder
                {
                    Id = x.Id,
                    StoreId = x.StoreId,
                    Region = x.Region,
                    Number=x.Number,
                    Remark= x.Remark,
                    ExpressAddress = x.ExpressAddress,
                    ExpressRegion = x.ExpressRegion,
                    ExpressMobile = x.ExpressMobile,
                    ExpressName = x.ExpressName,
                    ExpressZip = x.ExpressZip,
                    CreatedOn = x.CreatedOn,
                    Total = x.Total,
                    Status = x.Status.ToDescription(),
                    StoreOrderGoodses=x.StoreOrderGoodses.Select(z=>new StoreOrderGoods {
                        Id=z.Id,
                        GoodsId=z.GoodsId,
                        SpecificationId=z.SpecificationId,
                        SpecificationName=z.SpecificationName,
                        GoodsName=z.GoodsName,
                        GoodsPic=z.GoodsPic,
                        Quantity=z.Quantity,
                        Price=z.Price,
                        Total=z.Total
                    }).ToList()
                }).ToList()
            };
        }

        /// <summary>
        /// 商家的订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StoreOrder/StoreOrders")]
        public BaseApiResponse StoreOrders(StoreOrdersRequest request)
        {
            TryInitUserModel();
            int pageRecordCount = 10;
            //获取指定状态的订单
            IEnumerable<ReadModel.StoreOrders.Dtos.StoreOrderDetails> storeOrders = null;
            if (request.Status == StoreOrderStatus.All)
            {
                storeOrders = _storeOrderQueryService.StoreStoreOrderDetails(request.Id).OrderByDescending(x => x.CreatedOn).Skip(pageRecordCount * request.Page).Take(pageRecordCount);
            }
            else
            {
                storeOrders = _storeOrderQueryService.StoreStoreOrderDetails(request.Id,request.Status).OrderByDescending(x => x.CreatedOn).Skip(pageRecordCount*request.Page).Take(pageRecordCount);
            }

            return new UserOrdersResponse
            {
                StoreOrders = storeOrders.Select(x => new StoreOrder
                {
                    Id = x.Id,
                    StoreId = x.StoreId,
                    Region = x.Region,
                    Number = x.Number,
                    Remark = x.Remark,
                    ExpressAddress = x.ExpressAddress,
                    ExpressRegion = x.ExpressRegion,
                    ExpressMobile = x.ExpressMobile,
                    ExpressName = x.ExpressName,
                    ExpressZip = x.ExpressZip,
                    CreatedOn = x.CreatedOn,
                    Total = x.Total,
                    Status = x.Status.ToDescription(),
                    StoreOrderGoodses = x.StoreOrderGoodses.Select(z => new StoreOrderGoods
                    {
                        Id = z.Id,
                        GoodsId = z.GoodsId,
                        SpecificationId = z.SpecificationId,
                        SpecificationName = z.SpecificationName,
                        GoodsName = z.GoodsName,
                        GoodsPic = z.GoodsPic,
                        Quantity = z.Quantity,
                        Price = z.Price,
                        Total = z.Total
                    }).ToList()
                }).ToList()
            };
        }
        #endregion

        #region 包裹服务


        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StoreOrder/Deliver")]
        public async Task<BaseApiResponse> Deliver(DeliverRequest request)
        {
            request.CheckNotNull(nameof(request));
            request.ExpressInfo.CheckNotNull(nameof(request.ExpressInfo));

            var command = new DeliverCommand(request.ExpressInfo.ExpressName,
                request.ExpressInfo.ExpressNumber)
            {
                AggregateRootId=request.Id
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }

            return new BaseApiResponse();
            
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StoreOrder/ConfirmDeliver")]
        public async Task<BaseApiResponse> ConfirmDeliver(StoreOrderOpRequest request)
        {
            request.CheckNotNull(nameof(request));
            var command = new ConfirmDeliverCommand
            {
                AggregateRootId = request.Id
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }

            return new BaseApiResponse();
        }

        /// <summary>
        /// 申请退款
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StoreOrder/ApplyRefund")]
        public async Task<BaseApiResponse> ApplyRefund(ApplyRefundRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new ApplyRefundCommand(request.Reason, request.RefundAmount)
            {
                AggregateRootId = request.Id
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }

            return new BaseApiResponse();
        }

        /// <summary>
        /// 申请退货退款
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StoreOrder/ApplyReturnAndRefund")]
        public async Task<BaseApiResponse> ApplyReturnAndRefund(ApplyRefundRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new ApplyReturnAndRefundCommand(request.Reason, request.RefundAmount)
            {
                AggregateRootId = request.Id
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }

            return new BaseApiResponse();
        }

        /// <summary>
        /// 同意退款
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StoreOrder/AgreeRefund")]
        public async Task<BaseApiResponse> AgreeRefund(StoreOrderOpRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new AgreeRefundCommand()
            {
                AggregateRootId = request.Id
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }

            return new BaseApiResponse();
        }

        /// <summary>
        /// 同意退货
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StoreOrder/AgreeReturn")]
        public async Task<BaseApiResponse> AgreeReturn(StoreOrderOpRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new AgreeReturnCommand()
            {
                AggregateRootId = request.Id
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }

            return new BaseApiResponse();
        }
        #endregion

        #region 私有方法


        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.EventHandled).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}