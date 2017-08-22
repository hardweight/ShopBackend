using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Goodses;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Goodses;
using Shop.Api.Models.Response.Store;
using Shop.Commands.Goodses;
using Shop.Commands.Goodses.Specifications;
using Shop.ReadModel.Goodses;
using Shop.ReadModel.Stores;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    [ApiAuthorizeFilter]
    [EnableCors(origins: "http://app.wftx666.com,http://localhost:51776,http://localhost:8080", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class GoodsController:BaseApiController
    {
        private ICommandService _commandService;//C端
        private StoreQueryService _storeQueryService;//Q端
        private GoodsQueryService _goodsQueryService;

        public GoodsController(ICommandService commandService,
            StoreQueryService storeQueryService, 
            GoodsQueryService goodsQueryService)
        {
            _commandService = commandService;
            _storeQueryService = storeQueryService;
            _goodsQueryService = goodsQueryService;
        }


        /// <summary>
        /// 产品列表页面
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Goods/GoodsList")]
        public BaseApiResponse GoodsList(GoodsListRequest request)
        {
            request.CheckNotNull(nameof(request));

            if(request.Type.Equals("Search"))
            {
                //搜索商品
                var goodses = _goodsQueryService.Search(request.Search).OrderByDescending(x=>x.Rate);//默认根据评分
                if (request.Sort.Equals("销量"))
                {
                    goodses = goodses.OrderByDescending(x => x.SellOut);//根据销量
                }
                else if(request.Sort.Equals("新品"))
                {
                    goodses = goodses.OrderByDescending(x => x.CreatedOn);//根据发布时间
                }
                return new GoodsListResponse
                {
                    Goodses = goodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics = x.Pics.Split("|", true).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice
                    }).ToList()
                };
            }
            if(request.Type.Equals("Category"))
            {
                var goodses = _goodsQueryService.CategoryGoodses(request.CategoryId);
                return new GoodsListResponse
                {
                    Goodses = goodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics = x.Pics.Split("|", true).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice
                    }).ToList()
                };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 首页产品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Goods/HomePageGoodses")]
        public BaseApiResponse HomePageGoodses()
        {
                var rateGoodses = _goodsQueryService.GoodRateGoodses(12);
                var newGoodses = _goodsQueryService.NewGoodses(12);
                var selloutGoodses = _goodsQueryService.GoodSellGoodses(12);

                return new HomePageGoodsesResponse
                {
                    RateGoodses = rateGoodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics = x.Pics.Split("|", true).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice
                    }).ToList(),
                    NewGoodses = newGoodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics = x.Pics.Split("|", true).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice
                    }).ToList(),
                    SellOutGoodses = selloutGoodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics = x.Pics.Split("|", true).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice
                    }).ToList()
                };
        }
        
        /// <summary>
        /// 商品详情页面
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Goods/GoodsInfo")]
        public BaseApiResponse GoodsInfo(GoodsInfoRequest request)
        {
            request.CheckNotNull(nameof(request));
            var goodsDetails = _goodsQueryService.GetGoodsDetails(request.Id);
            var specifications = _goodsQueryService.GetPublishedSpecifications(request.Id);
            var comments = _goodsQueryService.GetComments(5);
            if(goodsDetails==null)
            {
                return new BaseApiResponse { Code = 400, Message = "没有该产品" };
            }
            return new GoodsInfoResponse
            {
                GoodsDetails = new GoodsDetails
                {
                    Id = goodsDetails.Id,
                    StoreId = goodsDetails.StoreId,
                    Pics = goodsDetails.Pics.Split("|", true).ToList(),
                    Name = goodsDetails.Name,
                    Description = goodsDetails.Description,
                    Surrender=goodsDetails.Surrender,
                    Price = goodsDetails.Price,
                    OriginalPrice = goodsDetails.OriginalPrice,
                    Stock = goodsDetails.Stock,
                    Is7SalesReturn = goodsDetails.Is7SalesReturn,
                    IsInvoice = goodsDetails.IsInvoice,
                    IsPayOnDelivery = goodsDetails.IsPayOnDelivery,
                    Rate=goodsDetails.Rate,
                    QualityRate=goodsDetails.QualityRate,
                    ExpressRate=goodsDetails.ExpressRate,
                    DescribeRate=goodsDetails.DescribeRate,
                    PriceRate=goodsDetails.PriceRate,
                    RateCount=goodsDetails.RateCount,
                    Sort = goodsDetails.Sort
                },
                Specifications=specifications.Select(x=>new Specification {
                    Id=x.Id,
                    Name=x.Name,
                    Value=x.Value,
                    Price=x.Price,
                    OriginalPrice=x.OriginalPrice,
                    Thumb=x.Thumb,
                    Stock=x.Stock,
                    BarCode=x.BarCode,
                    Number=x.Number,
                    AvailableQuantity=x.AvailableQuantity
                }).ToList(),
                Comments=comments.Select(x=>new Comment {
                    Id=x.Id,
                    Rate=x.Rate,
                    UserName=x.UserName,
                    CreatedOn=x.CreatedOn,
                    Thumbs=x.Thumbs.Split("|",true).ToList(),
                    Body=x.Body
                }).ToList()
            };
        }

        #region 店铺商品管理
        /// <summary>
        /// 添加修改商品商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GoodsStoreAdmin/AddGoods")]
        public async Task<BaseApiResponse> AddGoods(AddGoodsRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();

            //创建商品
            var goodsId = GuidUtil.NewSequentialId();
            var   command = new CreateGoodsCommand(
                goodsId,
                request.StoreId,
                request.CategoryIds,
                request.Name,
                request.Description,
                request.Pics,
                request.OriginalPrice,
                request.Stock,
                request.IsPayOnDelivery,
                request.IsInvoice,
                request.Is7SalesReturn,
                request.Sort);
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            //初始化默认规格
            var thumb = "";
            if(request.Pics.Any())
            {
                thumb = request.Pics[0];
            }
            var command2 = new AddSpecificationCommand(
                goodsId,
                "默认规格",
                "默认规格",
                thumb,
                request.OriginalPrice*1.2M,
                request.Stock,
                request.OriginalPrice,
                "",
                "");
            var result2 = await ExecuteCommandAsync(command2);
            if (!result2.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new AddGoodsResponse {
                GoodsId=goodsId
            };
        }

        /// <summary>
        /// 店铺更新商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GoodsStoreAdmin/StoreUpdateGoods")]
        public async Task<BaseApiResponse> StoreUpdateGoods(AddGoodsRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();

            var  command = new StoreUpdateGoodsCommand(
                request.CategoryIds,
                request.Name,
                request.Description,
                request.Pics,
                request.OriginalPrice,
                request.Stock,
                request.IsPayOnDelivery,
                request.IsInvoice,
                request.Is7SalesReturn,
                request.Sort)
            {
                AggregateRootId = request.Id
            };
            
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            //更新默认规格
            var specification = _goodsQueryService.GetGoodsDefaultSpecification(request.Id);
            if (specification != null)
            {
                var thumb = specification.Thumb;
                if (request.Pics.Any())
                {
                    thumb = request.Pics[0];
                }
                //更新默认规格
                var command2 = new UpdateSpecificationCommand(
                    request.Id,
                    specification.Id,
                    "默认规格",
                    "默认规格",
                    thumb,
                    request.OriginalPrice * 1.2M,
                    request.OriginalPrice,
                    "",
                    "",
                    request.Stock);
                var result2 = await ExecuteCommandAsync(command2);
                if (!result2.IsSuccess())
                {
                    return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
                }
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 后台更新商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GoodsStoreAdmin/StoreUpdateGoods")]
        public async Task<BaseApiResponse> UpdateGoods(UpdateGoodsRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();

            var command = new UpdateGoodsCommand(
                request.Name,
                request.Description,
                request.Pics,
                request.Price,
                request.SellOut)
            {
                AggregateRootId = request.Id
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            //如果有默认规格更新默认规格
            var specification = _goodsQueryService.GetGoodsDefaultSpecification(request.Id);
            if (specification!=null)
            {
                //更新默认规格
                var command2 = new UpdateSpecificationCommand(
                    request.Id,
                    specification.Id,
                    "默认规格",
                    "默认规格",
                    "",
                    request.Price,
                    specification.OriginalPrice,
                    "",
                    "",
                    specification.Stock);
                var result2 = await ExecuteCommandAsync(command2);
                if (!result2.IsSuccess())
                {
                    return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
                }
            }
            return new BaseApiResponse();
        }
        /// <summary>
        /// 更新商品参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GoodsStoreAdmin/UpdateGoodsParams")]
        public async Task<BaseApiResponse> UpdateGoodsParams(UpdateGoodsParamsRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new UpdateParamsCommand(request.GoodsId,
                request.Params.Select(x => new Commands.Goodses.GoodsParamInfo(x.Name, x.Value)).ToList());

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 添加商品规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GoodsStoreAdmin/AddGoodsSpecifications")]
        public async Task<BaseApiResponse> AddGoodsSpecifications(AddGoodsSpecificationsRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new AddSpecificationsCommand(request.GoodsId,
                request.Specifications.Select(x => new Commands.Goodses.Specifications.SpecificationInfo(
                    x.Name.ExpandAndToString(),
                    x.Value.ExpandAndToString(),
                    x.Thumb,
                    x.Price,
                    x.OriginalPrice,
                    x.Number,
                    x.BarCode,
                    x.Stock)).ToList());
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 店铺所有商品 管理用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GoodsStoreAdmin/AllGoodses")]
        public BaseApiResponse AllGoodses()
        {
            TryInitUserModel();

            //店铺信息
            var storeInfo = _storeQueryService.InfoByUserId(_user.Id);
            var goodses = _goodsQueryService.GetStoreGoodses(storeInfo.Id);
            return new AllGoodsResponse
            {
                Total = goodses.Count(),
                Goodses = goodses.Select(x => new GoodsDetails
                {
                    Id = x.Id,
                    StoreId=x.StoreId,
                    Pics=x.Pics.Split("|",true).ToList(),
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    Stock=x.Stock,
                    OriginalPrice=x.OriginalPrice,
                    Surrender =x.Surrender,
                    Is7SalesReturn=x.Is7SalesReturn,
                    IsInvoice=x.IsInvoice,
                    IsPayOnDelivery=x.IsPayOnDelivery,
                    CreatedOn = x.CreatedOn,
                    Sort=x.Sort
                }).ToList()
            };
        }
        #endregion

        #region 总后台管理
        /// <summary>
        /// 所有商品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("GoodsAdmin/Goodses")]
        public BaseApiResponse Goodses()
        {
            var goodses = _goodsQueryService.Goodses();
            return new GoodsesResponse
            {
                Total = goodses.Count(),
                Goodses = goodses.Select(x => new GoodsDetails
                {
                    Id = x.Id,
                    StoreId = x.StoreId,
                    Pics = x.Pics.Split("|", true).ToList(),
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    Stock = x.Stock,
                    OriginalPrice = x.OriginalPrice,
                    Surrender = x.Surrender,
                    Is7SalesReturn = x.Is7SalesReturn,
                    IsInvoice = x.IsInvoice,
                    IsPayOnDelivery = x.IsPayOnDelivery,
                    CreatedOn = x.CreatedOn,
                    Sort = x.Sort
                }).ToList()
            };
        }

        /// <summary>
        /// 上架商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("GoodsAdmin/Publish")]
        public async Task<BaseApiResponse> Publish(PublishRequest request)
        {
            request.CheckNotNull(nameof(request));
            var command = new PublishGoodsCommand
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
        /// 下架商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("GoodsAdmin/UnPublish")]
        public async Task<BaseApiResponse> UnPublish(PublishRequest request)
        {
            request.CheckNotNull(nameof(request));
            var command = new UnpublishGoodsCommand
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
            return _commandService.ExecuteAsync(command, CommandReturnType.CommandExecuted).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}