using ENode.Commanding;
using Shop.Common.Enums;
using System;

namespace Shop.Commands.Users
{
    public class ApplyToRegionPartnerCommand : Command<Guid>
    {
        public string Region { get; private set; }
        public PartnerLevel Level { get; private set; }

        public ApplyToRegionPartnerCommand() { }
        public ApplyToRegionPartnerCommand(string region,PartnerLevel level)
        {
            Region = region;
            Level = level;
        }
    }
}
