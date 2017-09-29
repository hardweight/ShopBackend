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
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("StoreOrder/Delete")]
        public async Task<BaseApiResponse> Delete(DeleteRequest request)
        {
            request.CheckNotNull(nameof(request));
            //判断
            var order = _storeOrderQueryService.Find(request.Id);
            if (order == null)
            {
                return new BaseApiResponse { Code = 400, Message = "没找到该订单" };
            }
            //删除
            var command = new DeleteStoreOrderCommand(request.Id);
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "删除失败{0}，可能订单状态不允许删除".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
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

            var expressSchedule = ExpressScheduleUtil.GetSchedule(request.ExpressName, request.ExpressNumber);

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
            request.CheckNotNull(nameof(request));

            TryInitUserModel();

            //获取数据
            int pageSize = 10;
            var storeOrders = _storeOrderQueryService.UserStoreOrderDetails(_user.Id);
            var total = storeOrders.Count();
            //筛选数据
            if (request.Status!=StoreOrderStatus.All)
            {
                storeOrders = storeOrders.Where(x=>x.Status==request.Status);
            }
            total = storeOrders.Count();
            //分页
            storeOrders = storeOrders.OrderByDescending(x=>x.CreatedOn).Skip(pageSize * (request.Page-1)).Take(pageSize);

            return new UserOrdersResponse
            {
                Total = total,
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

                    DeliverExpressName=x.DeliverExpressName,
                    DeliverExpressCode=x.DeliverExpressCode,
                    DeliverExpressNumber=x.DeliverExpressNumber,

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
            request.CheckNotNull(nameof(request));
            
            TryInitUserModel();

            //获取数据
            int pageSize = 10;
            var storeOrders = _storeOrderQueryService.StoreStoreOrderDetails(request.Id);
            var total = storeOrders.Count();
            //筛选数据
            if (request.Status != StoreOrderStatus.All)
            {
                storeOrders = storeOrders.Where(x => x.Status == request.Status);
            }
            //订单号查询
            if (!string.IsNullOrEmpty(request.OrderNumber))
            {
                storeOrders = storeOrders.Where(x => x.Number.Contains(request.OrderNumber));
            }
            total = storeOrders.Count();
            //分页
            storeOrders = storeOrders.OrderByDescending(x => x.CreatedOn).Skip(pageSize * (request.Page-1)).Take(pageSize);

            return new UserOrdersResponse
            {
                Total=total,
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

                    DeliverExpressName=x.DeliverExpressName,
                    DeliverExpressCode=x.DeliverExpressCode,
                    DeliverExpressNumber=x.DeliverExpressNumber,

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

            var command = new DeliverCommand(request.ExpressName,
                request.ExpressCode,
                request.ExpressNumber)
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

        #region 后台管理
        /// <summary>
        /// 店铺列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("StoreOrderAdmin/ListPage")]
        public ListPageResponse ListPage(ListPageRequest request)
        {
            request.CheckNotNull(nameof(request));

            var pageSize = 20;
            var storeOrders = _storeOrderQueryService.StoreOrderList();
            var total = storeOrders.Count();
            //筛选
            if (request.Status != StoreOrderStatus.All)
            {
                storeOrders = storeOrders.Where(x => x.Status == request.Status);
            }
            if (!request.Number.IsNullOrEmpty())
            {
                storeOrders = storeOrders.Where(x => x.Number.Contains(request.Number));
            }
            total = storeOrders.Count();

            //分页
            storeOrders = storeOrders.OrderByDescending(x => x.CreatedOn).Skip(pageSize * (request.Page - 1)).Take(pageSize);

            return new ListPageResponse
            {
                Total = total,
                StoreOrders = storeOrders.Select(x => new StoreOrderWithInfo
                {
                    Id = x.Id,
                    StoreId = x.StoreId,
                    Name=x.Name,
                    Mobile=x.Mobile,
                    NickName=x.NickName,
                    UserId=x.UserId,
                    Region = x.Region,
                    Number = x.Number,
                    Remark = x.Remark,
                    ExpressRegion = x.ExpressRegion,
                    ExpressAddress = x.ExpressAddress,
                    ExpressName = x.ExpressName,
                    ExpressMobile = x.ExpressMobile,
                    ExpressZip = x.ExpressZip,
                    CreatedOn = x.CreatedOn,

                    Total = x.Total,
                    StoreTotal = x.StoreTotal,
                    DeliverExpressName = x.DeliverExpressName,
                    DeliverExpressCode=x.DeliverExpressCode,
                    DeliverExpressNumber=x.DeliverExpressNumber,
                    Status = x.Status.ToString()
                }).ToList()
            };
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