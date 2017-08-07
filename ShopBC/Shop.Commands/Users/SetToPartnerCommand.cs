using ENode.Commanding;
using System;

namespace Shop.Commands.Users
{
    /// <summary>
    /// 设置用户为某个地区的 联盟
    /// </summary>
    public class SetToPartnerCommand:Command<Guid>
    {
        public string Region { get;private set; }
        public string Province { get; private set; }
        public string City { get; private set; }
        public string County { get; private set; }
        public PartnerLevel Level { get; private set; }

        public SetToPartnerCommand() { }
        public SetToPartnerCommand(string region,
            string province,
            string city,
            string county,
            PartnerLevel level)
        {
            Region = region;
            Province = province;
            City = city;
            County = county;
            Level = level;
        }
    }
}
