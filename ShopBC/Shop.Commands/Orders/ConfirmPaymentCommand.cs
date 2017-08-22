using System;
using ENode.Commanding;

namespace Shop.Commands.Orders
{
    public class ConfirmPaymentCommand : Command<Guid>
    {
        public bool IsPaymentSuccess { get; private set; }

        public ConfirmPaymentCommand() { }
        public ConfirmPaymentCommand(Guid orderId, bool isPaymentSuccess) : base(orderId)
        {
            IsPaymentSuccess = isPaymentSuccess;
        }
    }
}
