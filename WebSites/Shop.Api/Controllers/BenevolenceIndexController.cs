using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Helper;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.BenevolenceIndex;
using Shop.ReadModel.Stores;
using Shop.ReadModel.Wallets;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    [ApiAuthorizeFilter]
    [EnableCors(origins: "http://app.wftx666.com,http://localhost:51776,http://localhost:8080", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class BenevolenceIndexController:BaseApiController
    {
        private ICommandService _commandService;//C端
        private StoreQueryService _storeQueryService;//Q端
        private WalletQueryService _walletQueryService;//钱包Q端


        public BenevolenceIndexController(ICommandService commandService,
            StoreQueryService storeQueryService,
            WalletQueryService walletQueryService
            )
        {
            _commandService = commandService;
            _storeQueryService = storeQueryService;
            _walletQueryService = walletQueryService;
        }
        
        /// <summary>
        /// 获取此时的善心指数和统计信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("BenevolenceIndex/Info")]
        public BaseApiResponse Info()
        {
            //今日当前销售量
            var todaySale = _storeQueryService.TodaySale();
            //所有待分配的善心量
            var totalBenevolence = _walletQueryService.TotalBenevolence();
            decimal currentBIndex = 0;
            if (todaySale>0&&totalBenevolence>0)
            {
                currentBIndex = Math.Round((todaySale * 0.15M) / totalBenevolence, 4);
            }
            return new InfoResponse
            {
                CurrentBenevolenceIndex = currentBIndex,
                StoreCount = 44343,
                ConsumerCount = 3453,
                PasserCount = 35346,
                AmbassadorCount = 44343
            };
        }

        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.CommandExecuted).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}