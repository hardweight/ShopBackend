using System;
using ENode.Commanding;

namespace Buy.Commands.Orders
{
    [Serializable]
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
