using ENode.Commanding;
using System;

namespace Shop.Commands.Wallets.BenevolenceTransfers
{
    public class SetBenevolenceTransferSuccessCommand : Command<Guid>
    {
        public SetBenevolenceTransferSuccessCommand() { }
        public SetBenevolenceTransferSuccessCommand(Guid id) : base(id) { }
    }
}
