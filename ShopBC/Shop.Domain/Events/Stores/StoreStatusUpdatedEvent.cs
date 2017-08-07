using ENode.Eventing;
using Shop.Domain.Models.Stores;
using System;

namespace Shop.Domain.Events.Stores
{
    [Serializable]
    public class StoreStatusUpdatedEvent:DomainEvent<Guid>
    {
        public StoreStatus Status { get; private set; }

        public StoreStatusUpdatedEvent() { }
        public StoreStatusUpdatedEvent(StoreStatus status)
        {
            Status = status;
        }
    }
}
