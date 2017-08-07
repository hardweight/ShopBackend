using ENode.Eventing;
using Shop.Domain.Models.Stores;
using System;
using System.Collections.Generic;

namespace Shop.Domain.Events.Stores.StoreOrders
{
    /// <summary>
    /// 商家订单提交
    /// </summary>
    [Serializable]
    public class StoreOrderCreatedEvent:DomainEvent<Guid>
    {
        public Guid OrderId { get; private set; }
        public Guid StoreId { get;private set; }
        /// <summary>
        /// 这里应该是商家的最小地区 也就是商家保存的地区
        /// </summary>
        public string Region { get; private set; }
        public IList<OrderGoodsInfo> OrderGoodses { get; private set; }

        public StoreOrderCreatedEvent() { }
        public StoreOrderCreatedEvent(Guid orderId,Guid storeId,string region,IList<OrderGoodsInfo> orderGoodses)
        {
            OrderId = orderId;
            StoreId = storeId;
            Region = region;
            OrderGoodses = orderGoodses;
        }
    }
}
