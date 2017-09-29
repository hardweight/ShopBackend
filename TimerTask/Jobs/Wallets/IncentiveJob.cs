using Autofac;
using ECommon.Autofac;
using ECommon.Components;
using ENode.Commanding;
using Quartz;
using System.Linq;
using System;
using Shop.ReadModel.Wallets;
using Shop.Commands.Wallets;
using Shop.Common;

namespace Shop.TimerTask.Jobs.Wallets
{
    public class IncentiveJob: IJob
    {
        private ICommandService _commandService;//C端
        private WalletQueryService _walletQueryService;//Q 端
       

        public IncentiveJob()
        {
            var container = (ObjectContainer.Current as AutofacObjectContainer).Container;
            _commandService = container.Resolve<ICommandService>();
            _walletQueryService = container.Resolve<WalletQueryService>();
        }

        /// <summary>
        /// 计划任务
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            Incentive();
        }

        public void Incentive()
        {
            var benevolenceIndex = RandomArray.BenevolenceIndex();

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                //周日两倍激励
                benevolenceIndex = benevolenceIndex * 2;
            }

            //善心指数判断
            if (benevolenceIndex <= 0 || benevolenceIndex >= 1)
            {
                throw new Exception("善心指数异常");
            }

            //遍历所有的钱包发送激励指令
            var wallets = _walletQueryService.ListPage();
            foreach (var wallet in wallets)
            {
                var command = new IncentiveBenevolenceCommand(wallet.Id, benevolenceIndex);
                _commandService.SendAsync(command);
            }
        }
    }
}
