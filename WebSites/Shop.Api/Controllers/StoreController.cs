using ENode.Commanding;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Store;
using Shop.Api.Models.Response;
using Shop.ReadModel.Goodses;
using Shop.ReadModel.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Cors;
using Xia.Common.Extensions;
using Shop.Commands.Stores;
using ECommon.IO;
using Shop.Api.Extensions;
using System.Web.Http;
using Shop.Api.Models.Response.Store;

namespace Shop.Api.Controllers
{
    [ApiAuthorizeFilter]
    [EnableCors(origins: "http://localhost:51776,http://localhost:8080", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class StoreController:BaseApiController
    {
        private ICommandService _commandService;//C端

        private StoreQueryService _storeQueryService;//Q端
        private GoodsQueryService _goodsQueryService;
        
        public StoreController(ICommandService commandService, StoreQueryService storeQueryService,GoodsQueryService goodsQueryService)
        {
            _commandService = commandService;
            _storeQueryService = storeQueryService;
            _goodsQueryService = goodsQueryService;
        }

        #region 登录 创建
        [HttpPost]
        [Route("Store/ApplyStore")]
        public async Task<BaseApiResponse> ApplyStore(ApplyStoreRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();

            var command = new CreateStoreCommand(
                Guid.NewGuid(),
                _user.Id,
                "",
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

        /// <summary>
        /// 店铺信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Store/Info")]
        public StoreInfoResponse Info()
        {
            TryInitUserModel();
            //从缓存获取店铺信息
            var storeinfo = _apiSession.GetStoreInfo(_user.Id.ToString());

            return new StoreInfoResponse
            {
                StoreInfo = new StoreInfo
                {
                    Id = storeinfo.Id,
                    AccessCode = storeinfo.AccessCode,
                    Name = storeinfo.Name,
                    Description = storeinfo.Description,
                    Region = storeinfo.Region,
                    Address = storeinfo.Address
                }
            };
        }

        #region 商品管理

        #endregion

        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.CommandExecuted).TimeoutAfter(millisecondsDelay);
        }
        #endregion

    }
}