using System;

namespace Shop.ReadModel.Users.Dtos.UserGifts
{
    public class UserGift
    {
        public Guid Id { get; set; }
        public string GiftName { get; set; }
        public string GiftSize { get; set; }

        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }

        public string Remark { get; set; }
    }
}
