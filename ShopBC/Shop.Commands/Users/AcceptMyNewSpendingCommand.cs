using ENode.Commanding;
using System;

namespace Shop.Commands.Users
{
    public class AcceptMyNewSpendingCommand:Command<Guid>
    {
        public decimal Amount { get; private set; }
        public decimal Surrender { get; private set; }

        public AcceptMyNewSpendingCommand() { }
        public AcceptMyNewSpendingCommand(decimal amount,decimal surrender)
        {
            Amount = amount;
            Surrender = surrender;
        }
    }
}
