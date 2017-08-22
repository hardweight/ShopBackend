using ECommon.Components;
using ENode.Infrastructure;
using Shop.Domain.Events.Wallets.WithdrawApplys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommon.IO;
using ENode.Commanding;
using Shop.Commands.Wallets.CashTransfers;
using Xia.Common.Extensions;
using Shop.Common.Enums;
using Shop.Domain.Events.Wallets.RechargeApplys;
using Shop.Messages.Messages.Wallets;
using Shop.Commands.Wallets.BenevolenceTransfers;
using Xia.Common;

namespace Shop.ProcessManagers
{
    [Component]
    public class WalletProcessManager :
        IMessageHandler<WithdrawApplySuccessEvent>,
        IMessageHandler<RechargeApplySuccessEvent>,
        IMessageHandler<IncentiveUserBenevolenceMessage>//激励善心
    {
        private ICommandService _commandService;

        public WalletProcessManager(ICommandService commandService)
        {
            _commandService = commandService;
        }


        /// <summary>
        /// 提现申请审核通过，发送一个提现记录
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(WithdrawApplySuccessEvent evnt)
        {
            return _commandService.SendAsync(
                new CreateCashTransferCommand(
                    GuidUtil.NewSequentialId(),
                    evnt.AggregateRootId,
                    DateTime.Now.ToSerialNumber(),
                    CashTransferType.Withdraw,
                    CashTransferStatus.Placed,
                    evnt.Amount,
                    0,
                    WalletDirection.Out,
                    "提现申请成功"));
        }

        public Task<AsyncTaskResult> HandleAsync(RechargeApplySuccessEvent evnt)
        {
            return _commandService.SendAsync(
                new CreateCashTransferCommand(
                    GuidUtil.NewSequentialId(),
                    evnt.AggregateRootId,
                    DateTime.Now.ToSerialNumber(),
                    CashTransferType.Charge,
                    CashTransferStatus.Placed,
                    evnt.Amount,
                    0,
                    WalletDirection.In,
                    "线下充值申请成功"));
        }

        /// <summary>
        /// 激励用户善心
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(IncentiveUserBenevolenceMessage message)
        {
            //发布两个记录 一个现金记录  一个善心记录
            var tasks = new List<Task>();
            string number = DateTime.Now.ToSerialNumber();
            //现金记录
            tasks.Add(_commandService.SendAsync(new CreateCashTransferCommand(
                    GuidUtil.NewSequentialId(),
                    message.WalletId,
                    number,
                    CashTransferType.Incentive,
                    CashTransferStatus.Placed,
                    message.IncentiveValue,
                    0,
                    WalletDirection.In,
                    "善心激励")));
            //善心记录
            tasks.Add(_commandService.SendAsync(new CreateBenevolenceTransferCommand(
                    GuidUtil.NewSequentialId(),
                    message.WalletId,
                    number,
                    BenevolenceTransferType.Incentive,
                    BenevolenceTransferStatus.Placed,
                    message.BenevolenceDeduct,
                    0,
                    WalletDirection.Out,
                    "善心激励")));
            //执行所以的任务  
            Task.WaitAll(tasks.ToArray());
            return Task.FromResult(AsyncTaskResult.Success);
        }
    }
}
