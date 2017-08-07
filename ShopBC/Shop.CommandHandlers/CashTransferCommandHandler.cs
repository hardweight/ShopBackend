using ECommon.Components;
using ENode.Commanding;
using Shop.Commands.Wallets.CashTransfers;
using Shop.Domain.Models.Wallets.CashTransfers;

namespace Shop.CommandHandlers
{
    [Component]
    public class CashTransferCommandHandler :
        ICommandHandler<CreateCashTransferCommand>,
        ICommandHandler<SetCashTransferSuccessCommand>
    {
        public void Handle(ICommandContext context, CreateCashTransferCommand command)
        {
            var cashTransfer = new CashTransfer(
                command.AggregateRootId,
                command.WalletId,
                command.Number,
                new CashTransferInfo (command.Amount,
                command.Fee,
                command.Direction.ToWalletDirection(),
                command.Remark),
                command.Type.ToCashTransferType(),
                command.Status.ToCashTransferStatus());
            context.Add(cashTransfer);
        }

        public void Handle(ICommandContext context, SetCashTransferSuccessCommand command)
        {
            context.Get<CashTransfer>(command.AggregateRootId).SetStateSuccess();
        }
    }
}
