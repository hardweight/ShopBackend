using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Stores.StoreOrders.GoodsServices
{
    [Serializable]
    public class ServiceFinishedEvent : DomainEvent<Guid>
    {
        public decimal Total { get; private set; }
        public decimal SurrenderPersent { get; private set; }


        public ServiceFinishedEvent() { }
        public ServiceFinishedEvent(decimal total,decimal surrenderPersent)
        {
            Total = total;
            SurrenderPersent = surrenderPersent;
        }
    }
}
