using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Users
{
    /// <summary>
    /// 用户角色 升级为 传递使者
    /// </summary>
    [Serializable]
    public class UserRoleToPasserEvent:DomainEvent<Guid>
    {
        public UserRoleToPasserEvent() { }
    }
}
