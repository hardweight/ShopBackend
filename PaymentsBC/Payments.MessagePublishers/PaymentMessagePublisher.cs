using System.Threading.Tasks;
using ECommon.Components;
using ECommon.IO;
using ENode.Infrastructure;
using Payments.Messages;
using Payments.Events;

namespace Payments.MessagePublishers
{
    [Component]
    public class PaymentMessagePublisher :
        IMessageHandler<PaymentCompletedEvent>,
        IMessageHandler<PaymentRejectedEvent>
    {
        private readonly IMessagePublisher<IApplicationMessage> _messagePublisher;

        public PaymentMessagePublisher(IMessagePublisher<IApplicationMessage> messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public Task<AsyncTaskResult> HandleAsync(PaymentCompletedEvent evnt)
        {
            return _messagePublisher.PublishAsync(new PaymentCompletedMessage
            {
                PaymentId = evnt.AggregateRootId,
                OrderId = evnt.OrderId
            });
        }
        public Task<AsyncTaskResult> HandleAsync(PaymentRejectedEvent evnt)
        {
            return _messagePublisher.PublishAsync(new PaymentRejectedMessage
            {
                PaymentId = evnt.AggregateRootId,
                OrderId = evnt.OrderId
            });
        }
    }
}
