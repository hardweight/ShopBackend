using System;
using ENode.Infrastructure;

namespace Shop.Messages.Payments
{
    [Serializable]
    public abstract class PaymentMessage : ApplicationMessage
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
    }
}
