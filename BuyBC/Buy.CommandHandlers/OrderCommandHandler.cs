using Buy.Commands.Orders;
using Buy.Domain.Orders;
using Buy.Domain.Orders.Models;
using ECommon.Components;
using ENode.Commanding;
using System.Linq;

namespace Buy.CommandHandlers
{
    [Component]
    public class OrderCommandHandler :
        ICommandHandler<PlaceOrderCommand>,
        ICommandHandler<ConfirmOneReservationCommand>,
        ICommandHandler<ConfirmPaymentCommand>,
        ICommandHandler<MarkAsSuccessCommand>,
        ICommandHandler<CloseOrderCommand>
    {
        private readonly IPricingService _pricingService;

        public OrderCommandHandler(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        public void Handle(ICommandContext context, PlaceOrderCommand command)
        {
            context.Add(new Order(
                command.AggregateRootId,
                command.UserId,
                command.Specifications.Select(x => new SpecificationQuantity(new Specification(
                    x.SpecificationId,
                    x.GoodsId,
                    x.StoreId,
                    x.Name,
                    x.UnitPrice,
                    x.SurrenderPersent), x.Quantity)),
                _pricingService));
        }
        public void Handle(ICommandContext context, ConfirmOneReservationCommand command)
        {
            context.Get<Order>(command.AggregateRootId).ConfirmOneReservation( command.GoodsId,command.IsReservationSuccess);
        }
    
        public void Handle(ICommandContext context, ConfirmPaymentCommand command)
        {
            context.Get<Order>(command.AggregateRootId).ConfirmPayment(command.IsPaymentSuccess);
        }

        public void Handle(ICommandContext context, MarkAsSuccessCommand command)
        {
            context.Get<Order>(command.AggregateRootId).MarkAsSuccess(command.GoodsId);
        }
        public void Handle(ICommandContext context, CloseOrderCommand command)
        {
            context.Get<Order>(command.AggregateRootId).Close(command.GoodsId);
        }
    }
}
