using ENode.Infrastructure;
using System;

namespace Shop.Messages.Messages.Users
{
    [Serializable]
    public class IncentiveUserBenevolenceMessage: ApplicationMessage
    {
        public Guid WalletId { get;private set; }
        public Guid UserId { get; private set; }
        public decimal BenevolenceIndex { get; private set; }
        public decimal IncentiveValue { get; private set; }
        public decimal BenevolenceDeduct { get; private set; }

        public IncentiveUserBenevolenceMessage() { }
        public IncentiveUserBenevolenceMessage(
            Guid walletId,Guid userId,decimal benevolenceIndex,decimal incentiveValue,decimal benevolenceDeduct)
        {
            WalletId = walletId;
            UserId = userId;
            BenevolenceIndex = benevolenceIndex;
            IncentiveValue = incentiveValue;
            BenevolenceDeduct = benevolenceDeduct;
        }
    }
}
