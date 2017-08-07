using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Users
{
    [Serializable]
    public class UserUnFreezeEvent : DomainEvent<Guid>
    {
        public UserUnFreezeEvent() { }
    }
}
