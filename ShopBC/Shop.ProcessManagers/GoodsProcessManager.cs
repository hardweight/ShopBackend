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
using Shop.Domain.Events.Goodses;
using Shop.Commands.Goodses;

namespace Shop.ProcessManagers
{
    [Component]
    public class GoodsProcessManager :
        IMessageHandler<GoodsStoreUpdatedEvent>
    {
        private ICommandService _commandService;

        public GoodsProcessManager(ICommandService commandService)
        {
            _commandService = commandService;
        }


        /// <summary>
        /// 商家编辑商品 立即下架商品
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(GoodsStoreUpdatedEvent evnt)
        {
            return _commandService.SendAsync(
                new UnpublishGoodsCommand() {
                AggregateRootId=evnt.AggregateRootId});
        }
        
    }
}
