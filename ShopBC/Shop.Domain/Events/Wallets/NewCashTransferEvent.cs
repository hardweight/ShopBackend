using System;

namespace Shop.Domain.Events.Wallets
{
    [Serializable]
    public class NewCashTransferEvent:WalletEvent
    {
        public Guid TransferId { get; private set; }
        public decimal FinallyValue { get; private set; }

        public NewCashTransferEvent() { }
        public NewCashTransferEvent(Guid userId,Guid transferId,decimal finallyValue) :base(userId)
        {
            TransferId = transferId;
            FinallyValue = finallyValue;
        }
    }
}
