using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Stores.StoreOrders
{
    [Serializable]
    public class AgreeReturnEvent:DomainEvent<Guid>
    {
        public AgreeReturnEvent() { }
    }
}
