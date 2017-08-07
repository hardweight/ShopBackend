using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Users
{
    [Serializable]
    public class UserFreezeEvent : DomainEvent<Guid>
    {
        public UserFreezeEvent() { }
    }
}
