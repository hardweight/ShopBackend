using ENode.Commanding;
using System;

namespace Shop.Commands.Wallets.CashTransfers
{
    public class SetCashTransferSuccessCommand:Command<Guid>
    {
        public SetCashTransferSuccessCommand() { }
        public SetCashTransferSuccessCommand(Guid id) : base(id) { }
    }
}
