using ECommon.Components;
using ENode.Commanding;
using ENode.Infrastructure;
using System;
using Shop.Commands.Users;
using Shop.Domain.Services;
using Shop.Commands.Users.UserGifts;
using Shop.Commands.Users.ExpressAddresses;
using Shop.Domain.Models.Users;
using Shop.Domain.Models.Users.ExpressAddresses;
using Shop.Domain.Models.Users.UserGifts;
using Shop.Domain.Models.Wallets;

namespace Shop.CommandHandlers
{
    /// <summary>
    /// 用户领域模型 事件处理
    /// </summary>
    [Component]
    public class UserCommandHandler:
        ICommandHandler<CreateUserCommand>,
        ICommandHandler<SetMyParentCommand>,
        ICommandHandler<EditUserCommand>,
        ICommandHandler<UpdateNickNameCommand>,
        ICommandHandler<UpdatePasswordCommand>,
        ICommandHandler<UpdatePortraitCommand>,
        ICommandHandler<UpdateRegionCommand>,

        ICommandHandler<AddExpressAddressCommand>,
        ICommandHandler<UpdateExpressAddressCommand>,
        ICommandHandler<RemoveExpressAddressCommand>,

        ICommandHandler<AddUserGiftCommand>,
        ICommandHandler<SetUserGiftPayedCommand>,

        ICommandHandler<LockUserCommand>,
        ICommandHandler<UnLockUserCommand>,

        ICommandHandler<AcceptMyNewSpendingCommand>,//我的消费返还善心
        ICommandHandler<AcceptChildBenevolenceCommand>,
        ICommandHandler<ApplyToRegionPartnerCommand>,

        ICommandHandler<SetToPartnerCommand>,
        ICommandHandler<PayToAmbassadorCommand>,

        ICommandHandler<AcceptMyStoreNewSaleCommand>,
        ICommandHandler<GetChildStoreSaleBenevolenceCommand>
        
    {
        private readonly ILockService _lockService;
        private readonly RegisterUserMobileService _registerUserMobileService;

        /// <summary>
        /// IOC 构造函数注入
        /// </summary>
        /// <param name="lockService"></param>
        /// <param name="registerUserMobileService"></param>
        public UserCommandHandler(ILockService lockService, RegisterUserMobileService registerUserMobileService)
        {
            _lockService = lockService;
            _registerUserMobileService = registerUserMobileService;
        }

        #region handle Command
        public void Handle(ICommandContext context, CreateUserCommand command)
        {
            _lockService.ExecuteInLock(typeof(UserMobileIndex).Name, () =>
            {
                User parent = null;
                if (command.ParentId!=Guid.Empty)
                {
                    parent = context.Get<User>(command.ParentId);
                }
                //创建user 领域对象
                var user = new User(command.AggregateRootId, parent,new UserInfo(
                    command.Mobile,
                    command.NickName,
                    command.Portrait,
                    command.Gender,
                    command.Region,
                    command.Password,
                    command.WeixinId));

                //验证Mobile 的唯一性
                _registerUserMobileService.RegisterMobile(command.Id, user.Id, command.Mobile);
                //将领域对象添加到上下文中
                context.Add(user);
            });
        }

        public void Handle(ICommandContext context, SetMyParentCommand command)
        {
            User parent = null;
            if (command.ParentId != Guid.Empty)
            {
                parent = context.Get<User>(command.ParentId);
            }
            context.Get<User>(command.AggregateRootId).SetMyParent(parent);
        }

        public void Handle(ICommandContext context, UpdateNickNameCommand command)
        {
            //从上下文中获取User领域对象，然后直接调用领域对象的方法
            context.Get<User>(command.AggregateRootId).UpdateNickName(command.NickName);
        }

      

        public void Handle(ICommandContext context,UpdatePasswordCommand command)
        {
            context.Get<User>(command.AggregateRootId).UpdatePassword(command.Password);
        }

        public void Handle(ICommandContext context,UpdateGenderCommand command)
        {
            context.Get<User>(command.AggregateRootId).UpdateGender(command.Gender);
        }

        public void Handle(ICommandContext content,UpdatePortraitCommand command)
        {
            content.Get<User>(command.AggregateRootId).UpdatePortrait(command.Portrait);
        }
        public void Handle(ICommandContext context,UpdateRegionCommand command)
        {
            context.Get<User>(command.AggregateRootId).UpdateRegion(command.Region);
        }

    

        public void Handle(ICommandContext context,AddExpressAddressCommand command)
        {
            context.Get<User>(command.AggregateRootId).AddExpressAddress(new ExpressAddressInfo(
                command.Name,
                command.Mobile,
                command.Region,
                command.Address,
                command.Zip
                ));
        }

        public void Handle(ICommandContext context,UpdateExpressAddressCommand command)
        {
            context.Get<User>(command.AggregateRootId).UpdateExpressAddress(command.ExpressAddressId, new ExpressAddressInfo(
                command.Name,
                command.Mobile,
                command.Region,
                command.Address,
                command.Zip
                ));
        }

        public void Handle(ICommandContext context,RemoveExpressAddressCommand command)
        {
            context.Get<User>(command.AggregateRootId).RemoveExpressAddress(command.ExpressAddressId);
        }

    

        public void Handle(ICommandContext context,LockUserCommand command)
        {
            context.Get<User>(command.AggregateRootId).Lock();
        }
        public void Handle(ICommandContext context, UnLockUserCommand command)
        {
            context.Get<User>(command.AggregateRootId).UnLock();
        }

        public void Handle(ICommandContext context, AcceptMyNewSpendingCommand command)
        {
            var userId= context.Get<Wallet>(command.WalletId).GetOwnerId();
            context.Get<User>(userId).AcceptMyNewSpending(
                command.Amount,
                command.Benevolence);
        }

        public void Handle(ICommandContext context, AcceptChildBenevolenceCommand command)
        {
            context.Get<User>(command.AggregateRootId).AcceptChildBenevolence(command.Amount, command.Level);
        }

        public void Handle(ICommandContext context, ApplyToRegionPartnerCommand command)
        {
            context.Get<User>(command.AggregateRootId).ApplyToRegionPartner(command.Region, command.Level);
        }

        public void Handle(ICommandContext context, SetToPartnerCommand command)
        {
            context.Get<User>(command.AggregateRootId).SetToPartner(
                command.Region,
                command.Province,
                command.City,
                command.County,
                command.Level);
        }

        public void Handle(ICommandContext context, AddUserGiftCommand command)
        {
            context.Get<User>(command.AggregateRootId).AddUserGift(
                command.UserGiftId,
                new GiftInfo(command.GiftName, 
                command.GiftSize),
                new ExpressAddressInfo(
                    command.Name,
                    command.Mobile,
                    command.Region,
                    command.Address,
                    command.Zip));
        }

        public void Handle(ICommandContext context, PayToAmbassadorCommand command)
        {
            context.Get<User>(command.AggregateRootId).PayToAmbassador();
        }

        public void Handle(ICommandContext context, SetUserGiftPayedCommand command)
        {
            context.Get<User>(command.AggregateRootId).SetUserGiftPayed(command.UserGiftId);
        }

        public void Handle(ICommandContext context, EditUserCommand command)
        {
            context.Get<User>(command.AggregateRootId).Edit(command.NickName, command.Gender,command.Role);
        }

        public void Handle(ICommandContext context, AcceptMyStoreNewSaleCommand command)
        {
            var userId = context.Get<Wallet>(command.StoreOwnerWalletId).GetOwnerId();
            context.Get<User>(userId).AcceptMyStoreNewSale(command.Amount);
        }

        public void Handle(ICommandContext context, GetChildStoreSaleBenevolenceCommand command)
        {
            context.Get<User>(command.AggregateRootId).AcceptChildStoreSaleBenevolence(command.Amount);
        }



        #endregion
    }
}
