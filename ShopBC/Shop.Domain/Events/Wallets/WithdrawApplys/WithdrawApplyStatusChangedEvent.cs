using ENode.Eventing;
using Shop.Common.Enums;
using System;

namespace Shop.Domain.Events.Wallets.WithdrawApplys
{
    public class WithdrawApplyStatusChangedEvent:DomainEvent<Guid>
    {
        public Guid WithdrawApplyId { get; private set; }
        public WithdrawApplyStatus Status { get; private set; }
        public string Remark { get; private set; }

        public WithdrawApplyStatusChangedEvent() { }
        public WithdrawApplyStatusChangedEvent(Guid withdrawApplyId,
            WithdrawApplyStatus status,string remark)
        {
            WithdrawApplyId = withdrawApplyId;
            Status = status;
            Remark = remark;
        }
    }
}
