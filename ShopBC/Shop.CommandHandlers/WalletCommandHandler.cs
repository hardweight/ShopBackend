using ECommon.Components;
using ENode.Commanding;
using Shop.Commands.Wallets;
using Shop.Commands.Wallets.BankCards;
using Shop.Domain.Models.Wallets;
using Shop.Domain.Models.Wallets.BankCards;
using Shop.Domain.Models.Wallets.BenevolenceTransfers;
using Shop.Domain.Models.Wallets.CashTransfers;

namespace Shop.CommandHandlers
{
    [Component]
    public class WalletCommandHandler:
        ICommandHandler<CreateWalletCommand>,
        ICommandHandler<AcceptNewCashTransferCommand>,
        ICommandHandler<AcceptNewBenevolenceTransferCommand>,
        ICommandHandler<AddBankCardCommand>,
        ICommandHandler<UpdateBankCardCommand>,
        ICommandHandler<RemoveBankCardCommand>,
        ICommandHandler<SetAccessCodeCommand>
    {
        public void Handle(ICommandContext context, CreateWalletCommand command)
        {
            var wallet = new Wallet(command.AggregateRootId, command.UserId);
            context.Add(wallet);
        }
        public void Handle(ICommandContext context, AcceptNewCashTransferCommand command)
        {
            context.Get<Wallet>(command.AggregateRootId).AcceptNewCashTransfer(context.Get<CashTransfer>(command.TransferId));
        }
        public void Handle(ICommandContext context, AcceptNewBenevolenceTransferCommand command)
        {
            context.Get<Wallet>(command.AggregateRootId).AcceptNewBenevolenceTransfer(context.Get<BenevolenceTransfer>(command.TransferId));
        }

        public void Handle(ICommandContext context, AddBankCardCommand command)
        {
            context.Get<Wallet>(command.AggregateRootId).AddBankCard(new BankCardInfo(
                command.BankName,
                command.OwnerName,
                command.Number));
        }

        public void Handle(ICommandContext context, UpdateBankCardCommand command)
        {
            context.Get<Wallet>(command.AggregateRootId).UpdateBankCard(command.BankCardId, new BankCardInfo(
                command.BankName,
                command.OwnerName,
                command.Number));
        }

        public void Handle(ICommandContext context, RemoveBankCardCommand command)
        {
            context.Get<Wallet>(command.AggregateRootId).RemoveBankCard(command.BankCardId);
        }

        public void Handle(ICommandContext context, SetAccessCodeCommand command)
        {
            context.Get<Wallet>(command.AggregateRootId).SetAccessCode(command.AccessCode);
        }
    }
}
