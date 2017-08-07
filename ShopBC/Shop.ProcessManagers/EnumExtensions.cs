using Shop.Domain.Models.Partners;
using System;

namespace Shop.ProcessManagers
{
    public static  class EnumExtensions
    {
        public static Commands.Users.PartnerLevel ToCommandPartnerLevel(this PartnerLevel source)
        {
            var value = Commands.Users.PartnerLevel.Province;
            Enum.TryParse(source.ToString(), out value);
            return value;
        }
    }
}
