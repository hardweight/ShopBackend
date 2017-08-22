using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Stores.StoreOrders
{
    [Serializable]
    public class AgreeRefundedEvent:DomainEvent<Guid>
    {
        public Guid WalletId { get; set; }
        public decimal RefundAmount { get; set; }

        public AgreeRefundedEvent() { }
        public AgreeRefundedEvent(Guid walletId,decimal refundAmount)
        {
            WalletId = walletId;
            RefundAmount = refundAmount;
        }
    }
}
