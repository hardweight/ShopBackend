using ENode.Commanding;
using System;

namespace Shop.Commands.Users
{
    public class AcceptMyNewSpendingCommand:Command<Guid>
    {
        public decimal Amount { get; private set; }
        public decimal SurrenderPersent { get; private set; }

        public AcceptMyNewSpendingCommand() { }
        public AcceptMyNewSpendingCommand(decimal amount,decimal surrenderPersent)
        {
            Amount = amount;
            SurrenderPersent = surrenderPersent;
        }
    }
}
