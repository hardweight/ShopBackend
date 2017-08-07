using ENode.Eventing;
using Shop.Domain.Models.Partners;
using System;

namespace Shop.Domain.Events.Partners
{
    /// <summary>
    /// 父级联盟应该更新消费额的事件
    /// </summary>
    [Serializable]
    public class ParentPartnerShouldAcceptNewSaleEvent:DomainEvent<Guid>
    {
        public string Region { get; private set; }
        public decimal Amount { get; private set; }
        public PartnerLevel Level { get; private set; }

        public ParentPartnerShouldAcceptNewSaleEvent() { }
        public ParentPartnerShouldAcceptNewSaleEvent(string region,decimal amount,PartnerLevel level)
        {
            Region = region;
            Amount = amount;
            Level = level;
        }
    }
}
