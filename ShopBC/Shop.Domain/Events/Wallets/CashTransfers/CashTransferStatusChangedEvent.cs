using ENode.Eventing;
using Shop.Domain.Models.Wallets.CashTransfers;
using System;

namespace Shop.Domain.Events.Wallets.CashTransfers
{
    [Serializable]
    public class CashTransferStatusChangedEvent:DomainEvent<Guid>
    {
        public CashTransferStatus Status { get; private set; }

        public CashTransferStatusChangedEvent() { }
        public CashTransferStatusChangedEvent(CashTransferStatus status)
        {
            Status = status;
        }
    }
}
