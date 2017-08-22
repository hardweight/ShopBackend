using ENode.Eventing;
using Shop.Common.Enums;
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
