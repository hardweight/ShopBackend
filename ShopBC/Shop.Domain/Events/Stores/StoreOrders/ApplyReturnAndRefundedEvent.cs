using ENode.Eventing;
using Shop.Domain.Models.Stores.StoreOrders;
using System;

namespace Shop.Domain.Events.Stores.StoreOrders
{
    [Serializable]
    public class ApplyReturnAndRefundedEvent : DomainEvent<Guid>
    {
        public RefoundApplyInfo RefoundApplyInfo { get; set; }

        public ApplyReturnAndRefundedEvent() { }
        public ApplyReturnAndRefundedEvent(RefoundApplyInfo refoundApplyInfo)
        {
            RefoundApplyInfo = refoundApplyInfo;
        }
    }
}
