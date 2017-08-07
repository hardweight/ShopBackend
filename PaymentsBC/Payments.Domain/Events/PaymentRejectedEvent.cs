using System;
using ENode.Eventing;

namespace Payments.Events
{
    [Serializable]
    public class PaymentRejectedEvent : DomainEvent<Guid>
    {
        public Guid OrderId { get; private set; }

        public PaymentRejectedEvent() { }
        public PaymentRejectedEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
