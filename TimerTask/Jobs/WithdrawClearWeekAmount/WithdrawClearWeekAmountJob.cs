using Autofac;
using ECommon.Autofac;
using ECommon.Components;
using ENode.Commanding;
using Quartz;
using Shop.Commands.Wallets;
using Shop.ReadModel.Wallets;
using System.Diagnostics;
using System.Linq;

namespace Shop.TimerTask.Jobs.WithdrawClearWeekAmount
{
    /// <summary>
    /// 预订单任务
    /// </summary>
    public class WithdrawClearWeekAmountJob : IJob
    {
        private ICommandService _commandService;//C端
        private WalletQueryService _walletQueryService;//Q 端

        public WithdrawClearWeekAmountJob()
        {
            var container = (ObjectContainer.Current as AutofacObjectContainer).Container;
            _commandService = container.Resolve<ICommandService>();
            _walletQueryService = container.Resolve<WalletQueryService>();
        }
        
        /// <summary>
        /// 计划任务
        /// </summary>
        /// <param name="context"></param>
        public  void Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("任务执行了");
            Process();
        }


        private void Process()
        {
            var wallets = _walletQueryService.AllWallets();
            if (wallets.Any())
            {
                foreach (var wallet in wallets)
                {
                    var command = new ClearWeekWithdrawAmountCommand() {
                       AggregateRootId=wallet.Id
                    };
                    _commandService.SendAsync(command);
                }
            }
        }
    }
}