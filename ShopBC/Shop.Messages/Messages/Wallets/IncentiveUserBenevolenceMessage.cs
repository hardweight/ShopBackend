using ENode.Infrastructure;
using System;

namespace Shop.Messages.Messages.Wallets
{
    [Serializable]
    public class IncentiveUserBenevolenceMessage: ApplicationMessage
    {
        public Guid WalletId { get;private set; }
        public decimal BenevolenceIndex { get; private set; }
        public decimal IncentiveValue { get; private set; }
        public decimal BenevolenceDeduct { get; private set; }

        public IncentiveUserBenevolenceMessage() { }
        public IncentiveUserBenevolenceMessage(
            Guid walletId,decimal benevolenceIndex,decimal incentiveValue,decimal benevolenceDeduct)
        {
            WalletId = walletId;
            BenevolenceIndex = benevolenceIndex;
            IncentiveValue = incentiveValue;
            BenevolenceDeduct = benevolenceDeduct;
        }
    }
}
