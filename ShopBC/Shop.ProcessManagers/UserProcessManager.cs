using ECommon.Components;
using ECommon.IO;
using ENode.Commanding;
using ENode.Infrastructure;
using Shop.Commands.Carts;
using Shop.Commands.Partners;
using Shop.Commands.Users;
using Shop.Commands.Wallets;
using Shop.Commands.Wallets.BenevolenceTransfers;
using Shop.Common.Enums;
using Shop.Domain.Events.Partners;
using Shop.Domain.Events.Users;
using Shop.Domain.Events.Users.UserGifts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xia.Common;
using Xia.Common.Extensions;

namespace Shop.ProcessManagers
{
    [Component]
    public class UserProcessManager :
        IMessageHandler<UserCreatedEvent>,//创建用户
        IMessageHandler<UserSpendingTransformToBenevolenceEvent>,//用户消费转换为善心(User)
        IMessageHandler<UserGetSaleBenevolenceEvent>,//用户店铺销售激励善心
        IMessageHandler<MyParentCanGetBenevolenceEvent>,//我的父亲可以获得推荐善心奖励
        IMessageHandler<UserGetChildBenevolenceEvent>,//获取子的善心分成
        IMessageHandler<UserGetChildStoreSaleBenevolenceEvent>,//获取子商家销售分成奖励

        IMessageHandler<UserRoleToPartnerEvent>,//设置用户为联盟
        IMessageHandler<ParentPartnerShouldAcceptNewSaleEvent>,
        IMessageHandler<UserGiftPayedEvent>
       
    {
        private ICommandService _commandService;

        public UserProcessManager(ICommandService commandService)
        {
            _commandService = commandService;
        }
       
        /// <summary>
        /// 用户消费额转换为善心 用户剩余未转换逻辑已经实现，这里只需增加用户的善心就行了
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserSpendingTransformToBenevolenceEvent evnt)
        {
            var number = DateTime.Now.ToSerialNumber();
            return _commandService.SendAsync(new CreateBenevolenceTransferCommand(
                    GuidUtil.NewSequentialId(),
                    evnt.WalletId,
                    number,
                    BenevolenceTransferType.ShoppingAward,
                    BenevolenceTransferStatus.Success,
                    evnt.Amount,
                    0,
                    WalletDirection.In,
                    "购物激励"));
        }

      
        public Task<AsyncTaskResult> HandleAsync(UserGetSaleBenevolenceEvent evnt)
        {
            var number = DateTime.Now.ToSerialNumber();
            return _commandService.SendAsync(new CreateBenevolenceTransferCommand(
                    GuidUtil.NewSequentialId(),     
                    evnt.WalletId,
                    number,
                    BenevolenceTransferType.StoreAward,
                    BenevolenceTransferStatus.Success,
                    evnt.Amount,
                    0,
                    WalletDirection.In,
                    "店铺销售奖励"));
        }
        /// <summary>
        /// 我的父亲可以获得推荐善心奖励
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(MyParentCanGetBenevolenceEvent evnt)
        {
            return _commandService.SendAsync(new AcceptChildBenevolenceCommand(evnt.ParentId,evnt.Amount,evnt.Level));
        }
        /// <summary>
        /// 获取 子的善心分成
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserGetChildBenevolenceEvent evnt)
        {
            var number = DateTime.Now.ToSerialNumber();
            return _commandService.SendAsync(new CreateBenevolenceTransferCommand(
                    GuidUtil.NewSequentialId(),
                    evnt.WalletId,
                    number,
                    BenevolenceTransferType.RecommendUserAward,
                    BenevolenceTransferStatus.Success,
                    evnt.Amount,
                    0,
                    WalletDirection.In,
                    "推荐用户{0}度激励".FormatWith(evnt.Level)));
        }
        /// <summary>
        /// 推荐商家售货奖励
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserGetChildStoreSaleBenevolenceEvent evnt)
        {
            var number = DateTime.Now.ToSerialNumber();
            return _commandService.SendAsync(new CreateBenevolenceTransferCommand(
                    GuidUtil.NewSequentialId(),
                    evnt.WalletId,
                    number,
                    BenevolenceTransferType.RecommendStoreAward,
                    BenevolenceTransferStatus.Success,
                    evnt.Amount,
                    0,
                    WalletDirection.In,
                    "推荐商家售货激励"));
        }

        public Task<AsyncTaskResult> HandleAsync(UserRoleToPartnerEvent evnt)
        {
            //创建联盟聚合跟 需要根据级别初始化省市县信息
            return _commandService.SendAsync(new CreatePartnerCommand(
                GuidUtil.NewSequentialId(),
                evnt.UserId,
                evnt.WalletId,
                evnt.Region,
                evnt.Province,
                evnt.City,
                evnt.County,
                evnt.Level
                ));
        }

        public Task<AsyncTaskResult> HandleAsync(ParentPartnerShouldAcceptNewSaleEvent evnt)
        {
            //创建联盟聚合跟
            return _commandService.SendAsync(new Commands.Partners.AcceptNewSaleCommand(
                evnt.Region,
                evnt.Amount
                ));
        }

        /// <summary>
        /// 创建用户顺便创建用户的钱包信息
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserCreatedEvent evnt)
        {
            var tasks = new List<Task>();
            //创建用户的钱包信息
            tasks.Add( _commandService.SendAsync(new CreateWalletCommand(evnt.WalletId,
                evnt.AggregateRootId)));
            //创建用户的购物车信息
            tasks.Add(_commandService.SendAsync(new CreateCartCommand(evnt.CartId,
                evnt.AggregateRootId)));
            //执行所以的任务    
            //Task.WhenAll(tasks).ConfigureAwait(false); //验证失败
            Task.WaitAll(tasks.ToArray());
            return Task.FromResult(AsyncTaskResult.Success);
        }

        /// <summary>
        /// 传递大使 付款成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserGiftPayedEvent evnt)
        {
            return _commandService.SendAsync(
                new PayToAmbassadorCommand
                {
                    AggregateRootId=evnt.AggregateRootId
                });
        }
    }
}
