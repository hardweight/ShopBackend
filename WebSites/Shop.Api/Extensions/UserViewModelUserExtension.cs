using Shop.Api.ViewModels.User;
using Shop.Commands.Users;
using System;
using Xia.Common.Secutiry;
using Xia.Common.Extensions;
using Dtos = Shop.ReadModel.Users.Dtos;
using Shop.Commands.Users.ExpressAddresses;

namespace Shop.Api.Extensions
{
    public static class UserViewModelUserExtension
    {
        public static CreateUserCommand ToCreateUserCommand(this UserViewModel userViewModel)
        {
            var command = new CreateUserCommand(
                userViewModel.Id,
                userViewModel.ParentId,//推荐人id
                userViewModel.NickName,
                userViewModel.Portrait,
                userViewModel.Gender,
                userViewModel.Mobile,
                userViewModel.Region,
                PasswordHash.CreateHash(userViewModel.Password),
                "");
            command.AggregateRootId = userViewModel.Id;
            
            return command;
        }

        public static UserViewModel ToUserModel(this Dtos.User value)
        {
            return new UserViewModel()
            {
                Id = value.Id,
                ParentId=value.ParentId,
                WalletId=value.WalletId,
                CartId=value.CartId,
                Mobile = value.Mobile,
                Password = value.Password,
                NickName = value.NickName,
                Portrait = value.Portrait,
                Gender = value.Gender,
                Region = value.Region,
                Role=value.Role.ToDescription()
            };
        }

        public static AddExpressAddressCommand ToAddExpressAddressCommand(this ExpressAddressViewModel expressAddressViewModel)
        {
            var command = new AddExpressAddressCommand(expressAddressViewModel.UserId,
                expressAddressViewModel.Name,
                expressAddressViewModel.Mobile,
                expressAddressViewModel.Region,
                expressAddressViewModel.Address,
                expressAddressViewModel.Zip);

            return command;
        }
    }
}