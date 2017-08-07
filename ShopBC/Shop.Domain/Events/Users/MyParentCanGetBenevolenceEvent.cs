using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Users
{
    /// <summary>
    /// 我的父亲可以获得用户推荐分成的事件
    /// </summary>
    [Serializable]
    public class MyParentCanGetBenevolenceEvent:DomainEvent<Guid>
    {
        public Guid ParentId { get; private set; }
        public decimal Amount { get; private set; }
        public int Level { get; private set; }

        public MyParentCanGetBenevolenceEvent() { }
        public MyParentCanGetBenevolenceEvent(Guid parentId,decimal amount,int level)
        {
            ParentId = parentId;
            Amount = amount;
            Level = level;
        }
    }
}
