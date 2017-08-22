using ENode.Eventing;
using Shop.Common.Enums;
using Shop.Domain.Models.Wallets.WithdrawApplys;
using System;

namespace Shop.Domain.Events.Wallets.WithdrawApplys
{
    public class WithdrawApplyCreatedEvent:DomainEvent<Guid>
    {
        public Guid WithdrawApplyId { get; private set; }
        public WithdrawApplyInfo Info { get; private set; }
        public WithdrawApplyStatus Status { get; private set; }

        public WithdrawApplyCreatedEvent() { }
        public WithdrawApplyCreatedEvent(Guid withdrawApplyId,WithdrawApplyInfo info,WithdrawApplyStatus status)
        {
            WithdrawApplyId = withdrawApplyId;
            Info = info;
            Status = status;
        }
    }
}
