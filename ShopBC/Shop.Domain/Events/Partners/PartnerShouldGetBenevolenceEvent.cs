using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Partners
{
    /// <summary>
    /// 结算联盟 用户应该获取的善心量
    /// </summary>
    [Serializable]
    public class PartnerShouldGetBenevolenceEvent:DomainEvent<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid WalletId { get; private set; }
        public decimal Amount { get; private set; }

        public PartnerShouldGetBenevolenceEvent() { }
        public PartnerShouldGetBenevolenceEvent(Guid userId,Guid walletId,decimal amount)
        {
            UserId = userId;
            WalletId = walletId;
            Amount = amount;
        }
    }
}
