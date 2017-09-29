using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Store;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Goodses;
using Shop.Api.Models.Response.Store;
using Shop.Commands.Stores;
using Shop.Common.Enums;
using Shop.ReadModel.Goodses;
using Shop.ReadModel.StoreOrders;
using Shop.ReadModel.Stores;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    [ApiAuthorizeFilter]
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class StoreController:BaseApiController
    {
        private ICommandService _commandService;//C端

        private StoreQueryService _storeQueryService;//Q端
        private GoodsQueryService _goodsQueryService;
        private StoreOrderQueryService _storeOrderQueryService;
        
        public StoreController(ICommandService commandService, 
            StoreQueryService storeQueryService, 
            StoreOrderQueryService storeOrderQueryService,
            GoodsQueryService goodsQueryService)
        {
            _commandService = commandService;
            _storeQueryService = storeQueryService;
            _storeOrderQueryService = storeOrderQueryService;
            _goodsQueryService = goodsQueryService;
        }
        /// <summary>
        /// 店铺信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Store/Info")]
        public BaseApiResponse Info()
        {
            TryInitUserModel();

            var storeInfo = _storeQueryService.InfoByUserId(_user.Id);
            if (storeInfo == null)
            {
                return new BaseApiResponse();
            }
            //获取未发货订单
            var placedStoreOrderes = _storeOrderQueryService.StoreStoreOrderDetails(storeInfo.Id, StoreOrderStatus.Placed);
            if (storeInfo != null)
            {
                return new StoreInfoResponse
                {
                    StoreInfo = new StoreInfo
                    {
                        Id = storeInfo.Id,
                        AccessCode = storeInfo.AccessCode,
                        Name = storeInfo.Name,
                        Description = storeInfo.Description,
                        Region = storeInfo.Region,
                        Address = storeInfo.Address,
                        Type = storeInfo.Type.ToDescription(),
                        Status = storeInfo.Status.ToDescription()
                    },
                    SubjectInfo = new SubjectInfo
                    {
                        SubjectName = storeInfo.SubjectName,
                        SubjectNumber = storeInfo.SubjectNumber,
                        SubjectPic = storeInfo.SubjectPic
                    },
                    StatisticsInfo = new StatisticsInfo
                    {
                        TodaySale = storeInfo.TodaySale,
                        TodayOrder = storeInfo.TodayOrder,
                        TotalSale = storeInfo.TotalSale,
                        TotalOrder = storeInfo.TotalOrder
                    },
                    StoreOrders = placedStoreOrderes.Select(x => new Models.Response.StoreOrders.StoreOrder
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
                        StoreTotal = x.StoreTotal,
                        Status = x.Status.ToDescription(),
                        StoreOrderGoodses = x.StoreOrderGoodses.Select(z => new Models.Response.StoreOrders.StoreOrderGoods
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
                            StoreTotal = z.StoreTotal,
                        }).ToList()
                    }).ToList()
                };
            }
            else
            {
                return new BaseApiResponse { Code = 400, Message = "没有店铺" };
            }

        }

        /// <summary>
        /// 店铺信息首页
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Store/HomeInfo")]
        public BaseApiResponse HomeInfo(HomeInfoRequest request)
        {
            var storeInfo = _storeQueryService.Info(request.Id);
            if (storeInfo == null)
            {
                return new BaseApiResponse();
            }
            var goodses = _goodsQueryService.GetStoreGoodses(request.Id).OrderByDescending(x=>x.Rate).Take(60);
            if (storeInfo != null)
            {
                return new HomeInfoResponse
                {
                    StoreInfo = new StoreInfo
                    {
                        Id = storeInfo.Id,
                        AccessCode = storeInfo.AccessCode,
                        Name = storeInfo.Name,
                        Description = storeInfo.Description,
                        Region = storeInfo.Region,
                        Address = storeInfo.Address,
                        Type = storeInfo.Type.ToDescription(),
                        Status = storeInfo.Status.ToDescription()
                    },
                    SubjectInfo=new SubjectInfo
                    {
                        SubjectName=storeInfo.SubjectName,
                        SubjectNumber=storeInfo.SubjectNumber,
                        SubjectPic=storeInfo.SubjectPic
                    },
                    Goodses = goodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics= x.Pics.Split("|", true).Select(img => img.ToOssStyleUrl(OssImageStyles.GoodsMainPic.ToDescription())).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice,
                        Benevolence =x.Benevolence,
                        SellOut=x.SellOut
                    }).ToList()
                };
            }
            else
            {
                return new BaseApiResponse { Code = 400, Message = "没找到店铺" };
            }

        }

        #region 登录 创建
        [HttpPost]
        [Route("Store/ApplyStore")]
        public async Task<BaseApiResponse> ApplyStore(ApplyStoreRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();

            var command = new CreateStoreCommand(
                GuidUtil.NewSequentialId(),
                _user.Id,
                request.AccessCode,
                request.Name,
                request.Description,
                request.Region,
                request.Address,
                request.Subject.Name,
                request.Subject.Number,
                request.Subject.Pic);
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 设置管理密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Store/SetAccessCode")]
        public async Task<BaseApiResponse> SetAccessCode(SetAccessCodeRequest request)
        {
            request.CheckNotNull(nameof(request));

            TryInitUserModel();

            var command = new SetAccessCodeCommand(request.Id, request.AccessCode);

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }
        

        #endregion

        #region 店铺设置
        /// <summary>
        /// 编辑基本信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Store/Edit")]
        public async Task<BaseApiResponse> EditStore(EditRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new CustomerUpdateStoreCommand(
                request.Name,
                request.Description,
                request.Address)
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
        /// 编辑主体信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Store/EditSubject")]
        public async Task<BaseApiResponse> EditSubject(EditSubjectRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new UpdateSubjectCommand(
                request.SubjectName,
                request.SubjectNumber,
                request.SubjectPic)
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

        #region 总后台管理
        /// <summary>
        /// 店铺目前销售额
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("StoreAdmin/TotalTodaySale")]
        public BaseApiResponse TotalTodaySale()
        {
            var todaySale = _storeQueryService.TodaySale();
            return new TotalTodaySaleResponse
            {
                TotalTodaySale = todaySale
            };
        }
        
        /// <summary>
        /// 店铺列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("StoreAdmin/ListPage")]
        public ListPageResponse ListPage(ListPageRequest request)
        {
            request.CheckNotNull(nameof(request));

            var pageSize = 20;
            var stores = _storeQueryService.StoreList();
            var total = stores.Count();
            //筛选
            if (request.Type != StoreType.All)
            {
                stores = stores.Where(x => x.Type == request.Type);
            }
            if (request.Status != StoreStatus.All)
            {
                stores = stores.Where(x => x.Status == request.Status);
            }
            if (!request.Name.IsNullOrEmpty())
            {
                stores = stores.Where(x => x.Name.Contains(request.Name));
            }
            total = stores.Count();

            //分页
            stores = stores.OrderByDescending(x => x.CreatedOn).Skip(pageSize * (request.Page - 1)).Take(pageSize);
            
            return new ListPageResponse
            {
                Total = total,
                Stores = stores.Select(x => new Store
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Name = x.Name,
                    Region = x.Region,
                    Address = x.Address,
                    TodayOrder=x.TodayOrder,
                    OnSaleGoodsCount=x.OnSaleGoodsCount,
                    TodaySale=x.TodaySale,
                    Description=x.Description,
                    TotalOrder=x.TotalOrder,
                    TotalSale=x.TotalSale,

                    SubjectName=x.SubjectName,
                    SubjectNumber=x.SubjectNumber,
                    SubjectPic=x.SubjectPic,
                    Type=x.Type.ToString(),
                    Status = x.Status.ToString()
                }).ToList()
            };
        }

        /// <summary>
        /// 修改店铺信息 后台修改 可以修改店铺类型
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("StoreAdmin/Edit")]
        public async Task<BaseApiResponse> Edit(AdminEditRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new UpdateStoreCommand(
                request.Name,
                request.Description,
                request.Address,
                request.Type)
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
        /// 编辑商家状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("StoreAdmin/EditStatus")]
        public async Task<BaseApiResponse> EditStatus(EditStatusRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new UpdateStoreStautsCommand(
                request.Status)
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