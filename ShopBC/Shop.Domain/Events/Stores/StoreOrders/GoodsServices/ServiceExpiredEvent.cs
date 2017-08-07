using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Stores.StoreOrders.GoodsServices
{
    [Serializable]
    public class ServiceExpiredEvent:DomainEvent<Guid>
    {
        public decimal Total { get; private set; }
        public decimal SurrenderPersent { get; private set; }

        public ServiceExpiredEvent() { }
        public ServiceExpiredEvent(decimal total,decimal surrenderPersent)
        {
            Total = total;
            SurrenderPersent = surrenderPersent;
        }
    }
}
