using ECommon.Components;
using ECommon.IO;
using ENode.Commanding;
using ENode.Infrastructure;
using Shop.Commands.Stores;
using Shop.Commands.Stores.StoreOrders.OrderGoodses;
using Shop.Domain.Events.Stores.StoreOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.ProcessManagers
{
    [Component]
    public class StoreOrderProcessManager :
        IMessageHandler<StoreOrderCreatedEvent> //创建商家订单时
    {
        private ICommandService _commandService;

        public StoreOrderProcessManager(ICommandService commandService)
        {
            _commandService = commandService;
        }

        

        /// <summary>
        /// 创建商家订单  发送命令创建订单商品聚合跟 聚合跟的创建只能通过命令创建
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(StoreOrderCreatedEvent evnt)
        {
            var tasks = new List<Task>();
            //遍历订单商品分别发送创建订单命令
            foreach (var orderGoods in evnt.OrderGoodses)
            {
                tasks.Add(_commandService.SendAsync(new CreateOrderGoodsCommand(
                        Guid.NewGuid(),
                        evnt.StoreId,
                        evnt.OrderId,
                        new OrderGoods(
                            orderGoods.GoodsId,
                            orderGoods.SpecificationId,
                            orderGoods.SpecificationName,
                            orderGoods.Quantity,
                            orderGoods.Total,
                            orderGoods.SurrenderPersent))));
            }

            //发送联盟统计信息 联盟层级之间关系，交给联盟自己处理，这里不管
            tasks.Add(_commandService.SendAsync(new Commands.Partners.AcceptNewSaleCommand(
                evnt.Region,
                evnt.OrderGoodses.Sum(x=>x.Total)
                )));

            Task.WaitAll(tasks.ToArray());
            //Task.WhenAll(tasks).ConfigureAwait(false);
            return Task.FromResult(AsyncTaskResult.Success);
        }

    }
}
