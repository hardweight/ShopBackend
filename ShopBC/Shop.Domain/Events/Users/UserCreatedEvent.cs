using System;
using Shop.Domain.Models.Users;

namespace Shop.Domain.Events.Users
{
    /// <summary>
    /// 用户创建事件
    /// </summary>
    [Serializable]
    public class UserCreatedEvent : UserEvent
    {
        public Guid ParentId { get; private set; }

        public UserCreatedEvent() { }
        public UserCreatedEvent(UserInfo info,Guid parentId): base(info)
        {
            ParentId = parentId;
        }
    }
}
