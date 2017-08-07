using System;

namespace Shop.Domain.Events.Wallets
{
    [Serializable]
    public class NewBenevolenceTransferEvent:WalletEvent
    {
        public Guid TransferId { get; private set; }
        public decimal FinallyValue { get; private set; }

        public NewBenevolenceTransferEvent() { }
        public NewBenevolenceTransferEvent(Guid userId,Guid transferId, decimal finallyValue) :base(userId)
        {
            TransferId = transferId;
            FinallyValue = finallyValue;
        }
    }
}
