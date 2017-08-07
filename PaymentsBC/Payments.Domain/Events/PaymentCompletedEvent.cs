using System;
using ENode.Eventing;
using Payments.Domain.Modes;

namespace Payments.Events
{
    [Serializable]
    public class PaymentCompletedEvent : DomainEvent<Guid>
    {
        public Guid OrderId { get; private set; }

        public PaymentCompletedEvent() { }
        public PaymentCompletedEvent(Payment payment, Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
