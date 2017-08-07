using ENode.Commanding;
using System;

namespace Shop.Commands.Users.UserGifts
{
    public class AddUserGiftCommand: Command<Guid>
    {
        public Guid UserGiftId { get; private set; }
        public string GiftName { get; private set; }
        public string GiftSize { get; private set; }

        public string Name { get; private set; }
        public string Mobile { get; private set; }
        public string Region { get; private set; }
        public string Address { get; private set; }
        public string Zip { get; private set; }

        public AddUserGiftCommand() { }
        public AddUserGiftCommand(
            Guid userGiftId,
            Guid userId,
            string giftName,
            string giftSize,
            string name,
            string mobile,
            string region,
            string address,
            string zip):base(userId)
        {
            UserGiftId = userGiftId;
            GiftName = giftName;
            GiftSize = giftSize;

            Name = name;
            Mobile = mobile;
            Region = region;
            Address = address;
            Zip = zip;
        }
    }
}
