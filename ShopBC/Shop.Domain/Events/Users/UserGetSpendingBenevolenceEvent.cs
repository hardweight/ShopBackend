using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Users
{
    /// <summary>
    /// 用户获取 消费让利善心激励
    /// </summary>
    [Serializable]
    public class UserGetSpendingBenevolenceEvent : DomainEvent<Guid>
    {
        public Guid WalletId { get; private set; }
        public decimal Amount { get; private set; }

        public UserGetSpendingBenevolenceEvent() { }
        public UserGetSpendingBenevolenceEvent(Guid walletId,decimal amount)
        {
            WalletId = walletId;
            Amount = amount;
        }
    }
}
