using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Admins;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Admins;
using Shop.Api.Models.Response.Statisticses;
using Shop.ReadModel.StoreOrders;
using Shop.ReadModel.Stores;
using Shop.ReadModel.Users;
using Shop.ReadModel.Wallets;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common;
using Xia.Common.Extensions;
using System.Linq;
using Shop.Common.Enums;

namespace Shop.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class StatisticsController:BaseApiController
    {
        private ICommandService _commandService;//C端
        private UserQueryService _userQueryService;//用户Q端
        private WalletQueryService _walletQueryService;//钱包Q端
        private StoreOrderQueryService _storeOrderQueryService;//订单Q端
        private StoreQueryService _storeQueryService;//商家

        public StatisticsController(ICommandService commandService,
            UserQueryService userQueryService,
            WalletQueryService walletQueryService,
            StoreOrderQueryService storeOrderQueryService,
            StoreQueryService storeQueryService)
        {
            _commandService = commandService;
            _userQueryService = userQueryService;
            _walletQueryService = walletQueryService;
            _storeOrderQueryService = storeOrderQueryService;
            _storeQueryService = storeQueryService;
        }

        /// <summary>
        /// 仪表盘
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("StatisticsAdmin/DashBoard")]
        public BaseApiResponse DashBoard()
        {
            var users = _userQueryService.UserList();
            var wallets = _walletQueryService.AllWallets();
            var stores = _storeQueryService.StoreList();
            var incentiveBenevolenceTransfers = _walletQueryService.GetBenevolenceTransfers(BenevolenceTransferType.Incentive);

            return new DashBoardResponse
            {
                UserCount = users.Count(),
                NewRegCount = users.Where(x => x.CreatedOn > DateTime.Now.AddDays(-7)).Count(),
                AmbassadorCount = users.Where(x => x.Role == UserRole.Ambassador).Count(),
                NewAmbassadorCount = users.Where(x => x.CreatedOn > DateTime.Now.AddDays(-7) && x.Role == UserRole.Ambassador).Count(),

                CashTotal = wallets.Sum(x => x.Cash),
                LockedCashTotal = wallets.Sum(x => x.LockedCash),
                BenevolenceTotal = wallets.Sum(x => x.Benevolence),
                TodayBenevolenceAddedTotal = wallets.Sum(x => x.TodayBenevolenceAdded),
                LastIncentiveAmount= incentiveBenevolenceTransfers.Where(x=>x.CreatedOn.Date.Equals(DateTime.Now.Date)).Sum(x => x.Amount),
                TotalIncentiveAmount= incentiveBenevolenceTransfers.Sum(x=>x.Amount),

                TotalSale = stores.Sum(x => x.TotalSale),
                TodaySale = stores.Sum(x=>x.TodaySale),
                StoreOrderCount=stores.Sum(x=>x.TotalOrder),
                TodayStoreOrderCount=stores.Sum(x=>x.TodayOrder)
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