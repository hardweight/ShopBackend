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
using Shop.Common.Enums;
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
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
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
            int pageRecordCount = 10;
            if (request.Type.Equals("Search"))
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
                var pageGoodses = goodses.Skip(pageRecordCount * request.Page).Take(pageRecordCount);
                return new GoodsListResponse
                {
                    Goodses = pageGoodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics = x.Pics.Split("|", true).Select(img => img.ToOssStyleUrl(OssImageStyles.GoodsMainPic.ToDescription())).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice,
                        Benevolence = x.Benevolence
                    }).ToList()
                };
            }
            if(request.Type.Equals("Category"))
            {
                var goodses = _goodsQueryService.CategoryGoodses(request.CategoryId).Skip(pageRecordCount*request.Page).Take(pageRecordCount);
                return new GoodsListResponse
                {
                    Goodses = goodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics = x.Pics.Split("|", true).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice,
                        Benevolence = x.Benevolence
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
                var newGoodses = _goodsQueryService.NewGoodses(100);
                var selloutGoodses = _goodsQueryService.GoodSellGoodses(12);

                return new HomePageGoodsesResponse
                {
                    RateGoodses = rateGoodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics = x.Pics.Split("|", true).Select(img=>img.ToOssStyleUrl(OssImageStyles.GoodsMainPic.ToDescription())).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice,
                        Benevolence = x.Benevolence
                    }).ToList(),
                    NewGoodses = newGoodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics = x.Pics.Split("|", true).Select(img => img.ToOssStyleUrl(OssImageStyles.GoodsMainPic.ToDescription())).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice,
                        Benevolence = x.Benevolence
                    }).ToList(),
                    SellOutGoodses = selloutGoodses.Select(x => new Goods
                    {
                        Id = x.Id,
                        Pics = x.Pics.Split("|", true).Select(img => img.ToOssStyleUrl(OssImageStyles.GoodsMainPic.ToDescription())).ToList(),
                        Name = x.Name,
                        Price = x.Price,
                        OriginalPrice=x.OriginalPrice,
                        Benevolence = x.Benevolence
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
            var goodsParams = _goodsQueryService.GetGoodsParams(request.Id);
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
                    Pics = goodsDetails.Pics.Split("|", true).Select(img => img.ToOssStyleUrl(OssImageStyles.GoodsMainPic.ToDescription())).ToList(),
                    Name = goodsDetails.Name,
                    Description = goodsDetails.Description,
                    Benevolence = goodsDetails.Benevolence,
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
                    Benevolence=x.Benevolence,
                    Thumb=x.Thumb,
                    Stock=x.Stock,
                    BarCode=x.BarCode,
                    Number=x.Number,
                    AvailableQuantity=x.AvailableQuantity
                }).ToList(),
                GoodsParams=goodsParams.Select(x=>new GoodsParam {
                    Id=x.Id,
                    Name=x.Name,
                    Value=x.Value
                }).ToList(),
                Comments =comments.Select(x=>new Comment {
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
        /// 上架商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GoodsStoreAdmin/Publish")]
        public async Task<BaseApiResponse> GoodsPublish(PublishRequest request)
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
        [Route("GoodsStoreAdmin/UnPublish")]
        public async Task<BaseApiResponse> GoodsUnPublish(PublishRequest request)
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
                request.Price,
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
                request.Price,
                request.Stock,
                request.OriginalPrice,
                request.OriginalPrice/100M,
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
                request.Price,
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
                    request.Price,
                    request.OriginalPrice,
                    request.OriginalPrice/100M,
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
        /// 获取商品的参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GoodsStoreAdmin/GetParams")]
        public BaseApiResponse GetParams(GetParamsRequest request)
        {
            request.CheckNotNull(nameof(request));

            var goodsParams = _goodsQueryService.GetGoodsParams(request.Id);
            if (!goodsParams.Any())
            {
                return new BaseApiResponse { Code = 400, Message = "没有参数" };
            }
            return new GetParamsResponse
            {
                Params = goodsParams.Select(x => new GoodsParam
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value
                }).ToList()
            };
        }

        /// <summary>
        /// 获取规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GoodsStoreAdmin/GetSpecifications")]
        public BaseApiResponse StoreGetSpecifications(GetParamsRequest request)
        {
            request.CheckNotNull(nameof(request));

            var specifications = _goodsQueryService.GetPublishedSpecifications(request.Id);
            if (!specifications.Any())
            {
                return new BaseApiResponse { Code = 400, Message = "没有数据" };
            }
            return new GetSpecificationsResponse
            {
                Specifications = specifications.Select(x => new Specification
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value,
                    Price = x.Price,
                    OriginalPrice = x.OriginalPrice,
                    Benevolence=x.Benevolence,
                    Number = x.Number,
                    BarCode = x.BarCode,
                    Stock = x.Stock,
                    Thumb = x.Thumb
                }).ToList()
            };
        }

        /// <summary>
        /// 更新规格
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GoodsStoreAdmin/UpdateGoodsSpecifications")]
        public async Task<BaseApiResponse> UpdateGoodsSpecifications(UpdateGoodsSpecificationsRequest request)
        {
            request.CheckNotNull(nameof(request));

            //更新规格
            var command = new UpdateSpecificationsCommand(
                request.GoodsId,
                request.Specifications.Select(x => new Commands.Goodses.Specifications.SpecificationInfo(
                    x.Id,
                    x.Name,
                    x.Value,
                    x.Thumb,
                    x.Price,
                    x.OriginalPrice,
                    x.Benevolence,
                    x.Number,
                    x.BarCode,
                    x.Stock
                    )).ToList());
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            //改为未审核
            var command2 = new UpdateStatusCommand(request.GoodsId, GoodsStatus.UnVerify);
            var result2 = await ExecuteCommandAsync(command2);
            if (!result2.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result2.GetErrorMessage()) };
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
            //获取商品，现在商品的规格图片为商品的第一张图片
            var goodsAlis = _goodsQueryService.GetGoodsAlias(request.GoodsId);
            if(goodsAlis==null)
            {
                return new BaseApiResponse { Code = 400, Message = "没找到该商品" };
            }
            var thumbs = goodsAlis.Pics.Split("|", true);
            var thumb = "";
            if(thumbs.Any())
            {
                thumb = thumbs[0];
            }
            var command = new AddSpecificationsCommand(request.GoodsId,
                request.Specifications.Select(x => new Commands.Goodses.Specifications.SpecificationInfo(
                    GuidUtil.NewSequentialId(),
                    x.Name.ExpandAndToString(),
                    x.Value.ExpandAndToString(),
                    thumb,
                    x.Price,
                    x.Price,//客户端传过来的就是原价
                    x.Price/100M,
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

            var storeInfo = _storeQueryService.InfoByUserId(_user.Id);
            var goodses = _goodsQueryService.GetStoreGoodses(storeInfo.Id);
            return new AllGoodsResponse
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
                    Benevolence = x.Benevolence,
                    Is7SalesReturn = x.Is7SalesReturn,
                    IsInvoice = x.IsInvoice,
                    IsPayOnDelivery = x.IsPayOnDelivery,
                    CreatedOn = x.CreatedOn,
                    Sort = x.Sort,
                    IsPublished=x.IsPublished,
                    Status = x.Status.ToString()
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
        [Route("GoodsAdmin/ListPage")]
        public BaseApiResponse ListPage(ListPageRequest request)
        {
            request.CheckNotNull(nameof(request));

            var goodses = _goodsQueryService.Goodses().Where(x=>x.Status==request.Status);
            var pageSize = 20;
            var total = goodses.Count();
            if (!request.Name.IsNullOrEmpty())
            {
                goodses = goodses.Where(x => x.Name.Contains(request.Name)).OrderByDescending(x => x.CreatedOn).Skip(pageSize * (request.Page - 1)).Take(pageSize);
                total = goodses.Count();
            }
            else
            {
                goodses = goodses.OrderByDescending(x => x.CreatedOn).Skip(pageSize * (request.Page - 1)).Take(pageSize);
            }
            return new GoodsesResponse
            {
                Total = total,
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
                    Benevolence = x.Benevolence,
                    Is7SalesReturn = x.Is7SalesReturn,
                    IsInvoice = x.IsInvoice,
                    IsPayOnDelivery = x.IsPayOnDelivery,
                    CreatedOn = x.CreatedOn,
                    Sort = x.Sort,
                    IsPublished =x.IsPublished,
                    Status=x.Status.ToString()
                }).ToList()
            };
        }

        /// <summary>
        /// 后台更新商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("GoodsAdmin/UpdateGoods")]
        public async Task<BaseApiResponse> UpdateGoods(UpdateGoodsRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new UpdateGoodsCommand(
                request.Name,
                request.Description,
                request.Pics,
                request.Price,
                request.Benevolence,
                request.SellOut,
                request.Status)
            {
                AggregateRootId = request.Id
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "更新商品失败：{0}".FormatWith(result.GetErrorMessage()) };
            }
            //更新规格
            var command2 = new UpdateSpecificationsCommand(
                request.Id,
                request.Specifications.Select(x => new Commands.Goodses.Specifications.SpecificationInfo(
                    x.Id,
                    x.Name,
                    x.Value,
                    x.Thumb,
                    x.Price,
                    x.OriginalPrice,
                    x.Benevolence,
                    x.Number,
                    x.BarCode,
                    x.Stock
                    )).ToList());
            var result2 = await ExecuteCommandAsync(command2);
            if (!result2.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result2.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GoodsAdmin/GetSpecifications")]
        public BaseApiResponse GetSpecifications(GetParamsRequest request)
        {
            request.CheckNotNull(nameof(request));

            var specifications = _goodsQueryService.GetPublishedSpecifications(request.Id);
            if (!specifications.Any())
            {
                return new BaseApiResponse { Code = 400, Message = "没有参数" };
            }
            return new GetSpecificationsResponse
            {
                Specifications = specifications.Select(x => new Specification
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value,
                    Price = x.Price,
                    OriginalPrice = x.OriginalPrice,
                    Benevolence=x.Benevolence,
                    Number = x.Number,
                    BarCode = x.BarCode,
                    Stock = x.Stock,
                    Thumb = x.Thumb,
                    AvailableQuantity=x.AvailableQuantity
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
            return _commandService.ExecuteAsync(command, CommandReturnType.EventHandled).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}