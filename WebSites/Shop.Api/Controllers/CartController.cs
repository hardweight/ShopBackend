using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Carts;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Carts;
using Shop.Commands.Carts;
using Shop.ReadModel.Carts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    [ApiAuthorizeFilter]
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class CartController:BaseApiController
    {
        private ICommandService _commandService;//C端
        private CartQueryService _cartQueryService;//Q 端

        public CartController(ICommandService commandService, 
            CartQueryService cartQueryService)
        {
            _commandService = commandService;
            _cartQueryService = cartQueryService;
        }

        /// <summary>
        /// 我的购物车信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Cart/Info")]
        public BaseApiResponse Info()
        {
            TryInitUserModel();
            var cartGoodses = _cartQueryService.CartGoodses(_user.CartId);
            var storeCartGoodses = cartGoodses.GroupBy(x => x.StoreId).Select(g => new
            {
                StoreId = g.Key,
                Goodses = g.Select(gs => new
                {
                    Id = gs.Id,
                    GoodsId = gs.GoodsId,
                    StoreId = gs.StoreId,
                    SpecificationId = gs.SpecificationId,
                    StoreName=gs.StoreName,
                    SpecificationName = gs.SpecificationName,
                    GoodsName = gs.GoodsName,
                    GoodsPic=gs.GoodsPic,
                    Stock = gs.Stock,
                    Price = gs.Price,
                    OriginalPrice=gs.OriginalPrice,
                    Quantity = gs.Quantity,
                    Benevolence = gs.Benevolence
                })
            });

            return new CartInfoResponse
            {
                StoreCartGoods = storeCartGoodses.Select(x => new StoreCartGoods
                {
                    StoreId = x.StoreId,
                    StoreName = x.Goodses.First().StoreName,
                    CartGoodses = x.Goodses.Select(cg => new CartGoods
                    {
                        Id = cg.Id,
                        StoreId = cg.StoreId,
                        GoodsId = cg.GoodsId,
                        SpecificationId = cg.SpecificationId,
                        GoodsName = cg.GoodsName,
                        GoodsPic=cg.GoodsPic,
                        SpecificationName = cg.SpecificationName,
                        Price = cg.Price,
                        OriginalPrice=cg.OriginalPrice,
                        Quantity = cg.Quantity,
                        Stock = cg.Stock,
                        Benevolence = cg.Benevolence,
                        Checked=false
                    }).ToList()
                }).ToList()
            };
        }

        /// <summary>
        /// 添加购物车商品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Cart/AddCartGoods")]
        public async Task<BaseApiResponse> AddCartGoods(AddCartGoodsRequest request)
        {
            //获取用户的购物车后台获取
            request.CheckNotNull(nameof(request));

            TryInitUserModel();

            var command = new AddCartGoodsCommand(
                request.StoreId,
                request.GoodsId,
                request.SpecificationId,
                request.GoodsName,
                request.GoodsPic,
                request.SpecificationName,
                request.Price,
                request.OriginalPrice,
                request.Quantity,
                request.Stock,
                request.Benevolence)
            {
                AggregateRootId = _user.CartId
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 删除购物车商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Cart/RemoveCartGoods")]
        public async Task<BaseApiResponse> RemoveCartGoods(RemoveCartGoodsRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();

            var command = new RemoveCartGoodsCommand(request.Id)
            {
                AggregateRootId = _user.CartId
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.EventHandled).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}