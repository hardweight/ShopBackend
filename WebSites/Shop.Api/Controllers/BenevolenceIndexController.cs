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
using System.Linq;
using Shop.Common.Enums;

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
            
            //从缓存获取善心指数
            var benevolenceIndex = _apiSession.GetBenevolenceIndex();
            if (benevolenceIndex == null)
            {
                benevolenceIndex = RandomArray.BenevolenceIndex().ToString();
                _apiSession.SetBenevolenceIndex(benevolenceIndex);
            }
            currentBIndex = Convert.ToDecimal(benevolenceIndex);

            return new InfoResponse
            {
                CurrentBenevolenceIndex = currentBIndex,
                StoreCount = 3413,
                ConsumerCount = 3453,
                PasserCount = 35346,
                AmbassadorCount = 44343
            };
        }

        /// <summary>
        /// 善心排行榜
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("BenevolenceIndex/BenevolenceRank")]
        public BaseApiResponse BenevolenceRank()
        {
            var walletAlis = _walletQueryService.BenevolenceRank(10);

            return new BenevolenceRankResponse
            {
                WalletAlises = walletAlis.Select(x => new WalletAlis
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    NickName = x.NickName,
                    Mobile = x.Mobile,
                    Portrait=x.Portrait.ToOssStyleUrl(OssImageStyles.UserPortrait.ToDescription()),
                    Cash = x.Cash,
                    Benevolence = x.Benevolence,
                    Earnings = x.Earnings
                }).ToList()
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