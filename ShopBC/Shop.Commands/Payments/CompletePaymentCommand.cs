using System;
using ENode.Commanding;

namespace Shop.Commands.Payments
{
    public class CompletePaymentCommand : Command<Guid>
    {
        public CompletePaymentCommand() { }
    }
}
