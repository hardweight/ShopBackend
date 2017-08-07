using System.Linq;
using ECommon.Components;
using ENode.Commanding;
using Payments.Commands;
using Payments.Domain.Modes;

namespace Payments.CommandHandlers
{
    [Component]
    public class PaymentCommandHandler :
        ICommandHandler<CreatePaymentCommand>,
        ICommandHandler<CompletePaymentCommand>,
        ICommandHandler<CancelPaymentCommand>
    {
        public void Handle(ICommandContext context, CreatePaymentCommand command)
        {
            context.Add(new Payment(
                command.AggregateRootId,
                command.OrderId,
                command.Description,
                command.TotalAmount,
                command.Lines.Select(x => new PaymentItem(x.Description, x.Amount)).ToList()));
        }
        public void Handle(ICommandContext context, CompletePaymentCommand command)
        {
            context.Get<Payment>(command.AggregateRootId).Complete();
        }
        public void Handle(ICommandContext context, CancelPaymentCommand command)
        {
            context.Get<Payment>(command.AggregateRootId).Cancel();
        }
    }
}
