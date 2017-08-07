using ENode.Commanding;
using System;

namespace Shop.Commands.Users
{
    public class AcceptNewSaleCommand : Command<Guid>
    {
        public decimal Amount { get; private set; }
        public decimal SurrenderPersent { get; private set; }

        public AcceptNewSaleCommand() { }
        public AcceptNewSaleCommand(decimal amount,decimal surrenderPersent)
        {
            Amount = amount;
            SurrenderPersent = surrenderPersent;
        }
    }
}
