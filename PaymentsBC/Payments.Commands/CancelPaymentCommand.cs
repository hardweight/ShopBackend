using System;
using ENode.Commanding;

namespace Payments.Commands
{
    [Serializable]
    public class CancelPaymentCommand : Command<Guid>
    {
    }
}
