using System;
using System.Collections.Generic;
using ENode.Commanding;

namespace Shop.Commands.Payments
{
    public class CreatePaymentCommand : Command<Guid>
    {
        public Guid OrderId { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public IEnumerable<PaymentLine> Lines { get; set; }

        public CreatePaymentCommand() { }
        public CreatePaymentCommand(Guid orderId,string description,decimal totalAmount,IEnumerable<PaymentLine> lines)
        {
            OrderId = orderId;
            Description = description;
            TotalAmount = totalAmount;
            Lines = lines;
        }
    }
}
