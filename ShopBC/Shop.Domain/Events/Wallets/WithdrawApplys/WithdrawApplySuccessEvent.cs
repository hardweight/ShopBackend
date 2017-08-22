using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Wallets.WithdrawApplys
{
    public class WithdrawApplySuccessEvent:DomainEvent<Guid>
    {
        public decimal Amount { get; private set; }

        public WithdrawApplySuccessEvent() { }
        public WithdrawApplySuccessEvent(decimal amount)
        {
            Amount = amount;
        }
    }
}
