using ENode.Eventing;
using Shop.Domain.Models.Payments;
using System;

namespace Shop.Domain.Events.Payments
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
