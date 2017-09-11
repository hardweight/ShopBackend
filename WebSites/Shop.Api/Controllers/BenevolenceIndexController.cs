using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Helper;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.BenevolenceIndex;
using Shop.Common;
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
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
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
            //var todaySale = _storeQueryService.TodaySale();
            //所有待分配的善心量
            //var totalBenevolence = _walletQueryService.TotalBenevolence();
            decimal currentBIndex = 0;
            //if (todaySale>0&&totalBenevolence>0)
            //{
            //    currentBIndex = Math.Round((todaySale * 0.15M) / (totalBenevolence*ConfigSettings.BenevolenceValue*10), 4);
            //}

            //指定范围的随机数
            var randomArray =new decimal[] { 0.0023M, 0.00232M, 0.00234M, 0.00236M,0.00233M,0.00237M,
                0.0024M, 0.00241M, 0.00245M, 0.00242M,0.00243M,0.00247M,0.00248M,0.00249M,
                0.00256M, 0.00257M,0.00252M,0.00255M,0.00251M,0.00254M,
                0.0026M, 0.00261M, 0.00264M,0.00265M,0.00266M,0.00267M,0.00268M,0.00269M,
                0.0027M,0.00273M,0.00274M,0.00275M,0.00279M,
                0.0028M };
            currentBIndex = new Random().NextItem(randomArray);

            return new InfoResponse
            {
                CurrentBenevolenceIndex = currentBIndex,
                StoreCount = 3413,
                ConsumerCount = 3453,
                PasserCount = 35346,
                AmbassadorCount = 44343
            };
        }

        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.EventHandled).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}