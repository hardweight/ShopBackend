using ENode.Eventing;
using Shop.Common.Enums;
using Shop.Domain.Models.Partners;
using System;

namespace Shop.Domain.Events.Users
{
    /// <summary>
    /// 联盟申请已经提交
    /// </summary>
    [Serializable]
    public class RegionPartnerApplyedEvent:DomainEvent<Guid>
    {
        public string Region { get; private set; }
        public PartnerLevel Level { get; private set; }

        public RegionPartnerApplyedEvent() { }
        /// <summary>
        /// 联盟申请已经提交
        /// </summary>
        /// <param name="region"></param>
        /// <param name="level"></param>
        public RegionPartnerApplyedEvent(string region,PartnerLevel level)
        {
            Region = region;
            Level = level;
        }
    }
}
