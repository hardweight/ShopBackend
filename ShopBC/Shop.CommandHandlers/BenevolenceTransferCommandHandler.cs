using ECommon.Components;
using ENode.Commanding;
using Shop.Commands.Wallets.BenevolenceTransfers;
using Shop.Domain.Models.Wallets.BenevolenceTransfers;

namespace Shop.CommandHandlers
{
    [Component]
    public class BenevolenceTransferCommandHandler :
        ICommandHandler<CreateBenevolenceTransferCommand>,
        ICommandHandler<SetBenevolenceTransferSuccessCommand>
    {
        public void Handle(ICommandContext context, CreateBenevolenceTransferCommand command)
        {
            var cashTransfer = new BenevolenceTransfer(
                command.AggregateRootId,
                command.WalletId,
                command.Number,
                new BenevolenceTransferInfo (
                   command.Amount,
                    command.Fee,
                    command.Direction.ToWalletDirection(),
                   command.Remark
                ),
                command.Type.ToBenevolenceTransferType(),
                command.Status.ToBenevolenceTransferStatus());
            context.Add(cashTransfer);
        }

        public void Handle(ICommandContext context, SetBenevolenceTransferSuccessCommand command)
        {
            context.Get<BenevolenceTransfer>(command.AggregateRootId).SetStateSuccess();
        }
    }
}
