using ENode.Eventing;
using Shop.Common.Enums;
using Shop.Domain.Models.Partners;
using System;

namespace Shop.Domain.Events.Users
{
    /// <summary>
    /// 用户角色升级为 联盟
    /// </summary>
    [Serializable]
    public class UserRoleToPartnerEvent : DomainEvent<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid WalletId { get; private set; }
        public string Region { get; private set; }
        public string Province { get; private set; }
        public string City { get; private set; }
        public string County { get; private set; }

        public PartnerLevel Level { get; private set; }

        public UserRoleToPartnerEvent() { }
        public UserRoleToPartnerEvent(Guid userId,Guid walletId,string region,string province,string city,string county,PartnerLevel level)
        {
            UserId = userId;
            WalletId = walletId;
            Region = region;
            Province = province;
            City = city;
            County = county;
            Level = level;
        }
    }
}
