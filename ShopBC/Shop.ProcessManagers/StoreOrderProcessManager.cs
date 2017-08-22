using ECommon.Components;
using ECommon.IO;
using ENode.Commanding;
using ENode.Infrastructure;
using Shop.Commands.Partners;
using Shop.Commands.Stores;
using Shop.Commands.Stores.StoreOrders.OrderGoodses;
using Shop.Commands.Wallets.CashTransfers;
using Shop.Domain.Events.Stores.StoreOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xia.Common;
using Xia.Common.Extensions;

namespace Shop.ProcessManagers
{
    [Component]
    public class StoreOrderProcessManager :
        IMessageHandler<StoreOrderCreatedEvent>, //创建商家订单时
        IMessageHandler<StoreOrderConfirmExpressedEvent>,//用户确认收货，给商家结算
        IMessageHandler<AgreeRefundedEvent> //同意包裹退款
    {
        private ICommandService _commandService;

        public StoreOrderProcessManager(ICommandService commandService)
        {
            _commandService = commandService;
        }

        

        /// <summary>
        /// 创建商家订单  发送命令创建订单商品
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(StoreOrderCreatedEvent evnt)
        {
            var tasks = new List<Task>();
            //遍历订单商品分别发送创建订单商品命令
            foreach (var orderGoods in evnt.OrderGoodses)
            {
                tasks.Add(_commandService.SendAsync(new CreateOrderGoodsCommand(
                        GuidUtil.NewSequentialId(),
                        evnt.Info.StoreId,
                        evnt.AggregateRootId,
                        new OrderGoods(
                            orderGoods.GoodsId,
                            orderGoods.SpecificationId,
                            orderGoods.GoodsName,
                            orderGoods.GoodsPic,
                            orderGoods.SpecificationName,
                            orderGoods.Price,
                            orderGoods.OriginalPrice,
                            orderGoods.Quantity,
                            orderGoods.Total,
                            orderGoods.StoreTotal,
                            orderGoods.Surrender)
                        {
                            WalletId=orderGoods.WalletId,
                            StoreOwnerWalletId=orderGoods.StoreOwnerWalletId
                        })));
            }

            //发送给商家接受新的销售
            tasks.Add(_commandService.SendAsync(
                new AcceptNewStoreOrderCommand(evnt.Info.StoreId,
                evnt.AggregateRootId)));

            //发送联盟统计信息 联盟层级之间关系，交给联盟自己处理，这里不管 
            //tasks.Add(_commandService.SendAsync(new AcceptNewSaleCommand(
            //    evnt.Region,
            //    evnt.OrderGoodses.Sum(x => x.Total)
            //    )
            //{
            //    AggregateRootId=GuidUtil.NewSequentialId()
            //}));

            Task.WaitAll(tasks.ToArray());
            return Task.FromResult(AsyncTaskResult.Success);
        }

        public Task<AsyncTaskResult> HandleAsync(AgreeRefundedEvent evnt)
        {
            //发送退款指令
            return _commandService.SendAsync(new CreateCashTransferCommand(
                GuidUtil.NewSequentialId(),
                evnt.WalletId,
                DateTime.Now.ToSerialNumber(),
                Common.Enums.CashTransferType.Refund,
                Common.Enums.CashTransferStatus.Placed,
                evnt.RefundAmount,
                0,
                Common.Enums.WalletDirection.In,
                "订单退款"
                ));
        }

        /// <summary>
        /// 确认收货，系统给商家结算销售收入
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(StoreOrderConfirmExpressedEvent evnt)
        {
            //发送给商家所有人付款的指令
            return _commandService.SendAsync(new CreateCashTransferCommand(
                GuidUtil.NewSequentialId(),
                evnt.StoreOwnerWalletId,
                DateTime.Now.ToSerialNumber(),
                Common.Enums.CashTransferType.StoreSell,
                Common.Enums.CashTransferStatus.Placed,
                evnt.StoreGetAmount,
                0,
                Common.Enums.WalletDirection.In,
                "店铺销售商品"
                ));
        }
    }
}
