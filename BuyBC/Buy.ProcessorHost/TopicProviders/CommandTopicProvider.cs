using ECommon.Components;
using ENode.Commanding;
using ENode.EQueue;
using Payments.Commands;
using Shop.Commands.Goodses.Specifications;
using Shop.Common;

namespace Buy.ProcessorHost.TopicProviders
{
    [Component]
    public class CommandTopicProvider : AbstractTopicProvider<ICommand>
    {
        public override string GetTopic(ICommand command)
        {
            if (command is MakeSpecificationReservationCommand || command is CommitSpecificationReservationCommand || command is CancelSpecificationReservationCommand)
            {
                return Topics.ShopCommandTopic;
            }
            else if (command is CreatePaymentCommand || command is CompletePaymentCommand || command is CancelPaymentCommand)
            {
                return Topics.PaymentCommandTopic;
            }
            return Topics.BuyCommandTopic;
        }
    }
}
