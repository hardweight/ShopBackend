using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using Shop.Domain.Events.Stores.StoreOrders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.ReadModel.Stores
{
    /// <summary>
    /// 更新读库 Dapper
    /// </summary>
    [Component]
    public class StoreOrderViewModelGenerator:BaseGenerator,
        IMessageHandler<StoreOrderCreatedEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(StoreOrderCreatedEvent evnt)
        {
            return TryTransactionAsync((connection, transaction) =>
            {
                var tasks = new List<Task>();

                //插入订单主记录
                tasks.Add(connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    StoreId=evnt.StoreId,
                    CreatedOn = evnt.Timestamp,
                    Total = evnt.OrderGoodses.Sum(x=>x.Total),
                    Version = evnt.Version,
                    EventSequence=evnt.Sequence
                }, ConfigSettings.StoreOrderTable, transaction));

                //插入订单明细?这里应该用命令创建
                //foreach (var orderGoods in evnt.OrderGoodses)
                //{
                //    tasks.Add(connection.InsertAsync(new
                //    {
                //        OrderId = evnt.AggregateRootId,
                //        GoodsId=orderGoods.GoodsId,
                //        ExpirationDate= orderGoods.ExpirationDate,
                //        SpecificationId = orderGoods.SpecificationId,
                //        SpecificationName = orderGoods.SpecificationName,
                //        Quantity = orderGoods.Quantity,
                //        UnitPrice = orderGoods.UnitPrice,
                //        Total = orderGoods.Total
                //    }, ConfigSettings.OrderGoodsTable, transaction));
                //}
                return tasks;
            });
        }
        
    }
}
