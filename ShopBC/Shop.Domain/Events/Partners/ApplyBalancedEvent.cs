using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Partners
{
    [Serializable]
    public class ApplyBalancedEvent:DomainEvent<Guid>
    {
        public ApplyBalancedEvent() { }
    }
}
