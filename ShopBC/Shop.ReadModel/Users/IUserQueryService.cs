using Shop.ReadModel.Users.Dtos.ExpressAddresses;
using Shop.ReadModel.Users.Dtos.UserGifts;
using System;
using Shop.ReadModel.Users.Dtos;
using System.Collections.Generic;

namespace Shop.ReadModel.Users
{
    public interface IUserQueryService
    {
        
        User FindUser(Guid userId);
        User FindUser(string mobile);
        bool CheckMobileIsAvliable(string mobile);

        IEnumerable<ExpressAddress> GetExpressAddresses(Guid userId);
        IEnumerable<UserGift> UserGifts(Guid userId);
    }
}
