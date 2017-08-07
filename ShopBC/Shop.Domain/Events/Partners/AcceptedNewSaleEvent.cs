using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Partners
{
    /// <summary>
    /// 接受新的地区销售
    /// </summary>
    [Serializable]
    public class AcceptedNewSaleEvent:DomainEvent<Guid>
    {
        /// <summary>
        /// 最终数量
        /// </summary>
        public decimal Amount { get; private set; }

        public AcceptedNewSaleEvent() { }
        public AcceptedNewSaleEvent(decimal amount)
        {
            Amount = amount;
        }
    }
}
