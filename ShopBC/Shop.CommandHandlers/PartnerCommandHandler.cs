using ECommon.Components;
using ENode.Commanding;
using Shop.Commands.Partners;
using Shop.Domain.Models.Partners;

namespace Shop.CommandHandlers
{
    [Component]
    public class PartnerCommandHandler :
        ICommandHandler<CreatePartnerCommand>,
        ICommandHandler<ApplyBalanceCommand>,
        ICommandHandler<AcceptNewSaleCommand>
    {
        public void Handle(ICommandContext context, CreatePartnerCommand command)
        {
            context.Add(new Partner(
                command.AggregateRootId, 
                command.UserId,
                command.WalletId,
                command.Region,
                command.Province,
                command.City,
                command.Province,
                command.Level.ToPartnerLevel()));
        }
        public void Handle(ICommandContext context, ApplyBalanceCommand command)
        {
            context.Get<Partner>(command.AggregateRootId).ApplyBalance();
        }

        public void Handle(ICommandContext context, AcceptNewSaleCommand command)
        {
            context.Get<Partner>(command.AggregateRootId).AcceptNewSale(command.Region,command.Amount);
        }
    }
}
