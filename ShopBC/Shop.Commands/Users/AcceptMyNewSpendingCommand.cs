using ENode.Commanding;
using System;

namespace Shop.Commands.Users
{
    public class AcceptMyNewSpendingCommand:Command<Guid>
    {
        public Guid WalletId { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Benevolence { get; private set; }

        public AcceptMyNewSpendingCommand() { }
        public AcceptMyNewSpendingCommand(
            Guid  walletId,
            decimal amount,
            decimal benevolence)
        {
            WalletId = walletId;
            Amount = amount;
            Benevolence = benevolence;
        }
    }
}
