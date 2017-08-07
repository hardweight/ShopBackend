using ENode.Eventing;
using Shop.Domain.Models.Wallets.BenevolenceTransfers;
using System;

namespace Shop.Domain.Events.Wallets.BenevolenceTransfers
{
    [Serializable]
    public class BenevolenceTransferStatusChangedEvent : DomainEvent<Guid>
    {
        public BenevolenceTransferStatus Status { get; private set; }

        public BenevolenceTransferStatusChangedEvent() { }
        public BenevolenceTransferStatusChangedEvent(BenevolenceTransferStatus status)
        {
            Status = status;
        }
    }
}
