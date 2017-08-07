using ENode.Commanding;
using Shop.Commands.Users;
using System;

namespace Shop.Commands.Partners
{
    public class CreatePartnerCommand:Command<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid WalletId { get; private set; }
        public string Region { get; private set; }
        public string Province { get; private set; }
        public string City { get; private set; }
        public string County { get; private set; }
        public PartnerLevel Level { get; private set; }

        public CreatePartnerCommand() { }
        public CreatePartnerCommand(Guid id,
            Guid userId,
            Guid walletId,
            string region,
            string province,
            string city,
            string county,
            PartnerLevel level):base(id)
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
