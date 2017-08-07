using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Users
{
    /// <summary>
    /// 获取子的商家销售额善心分成
    /// </summary>
    [Serializable]
    public class UserGetChildStoreSaleBenevolenceEvent : DomainEvent<Guid>
    {
        public Guid WalletId { get; private set; }
        public decimal Amount { get; private set; }

        public UserGetChildStoreSaleBenevolenceEvent() { }
        public UserGetChildStoreSaleBenevolenceEvent(Guid walletId,decimal amount)
        {
            WalletId = walletId;
            Amount = amount;
        }
    }
}
