using System;

namespace Payments.Domain.Modes
{
    /// <summary>
    /// 支付项目
    /// </summary>
    public class PaymentItem
    {
        public PaymentItem(string description, decimal amount)
        {
            Id = Guid.NewGuid();
            Description = description;
            Amount = amount;
        }

        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
    }
}
