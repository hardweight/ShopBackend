using ECommon.Components;
using ENode.Commanding;
using ENode.Infrastructure;
using Shop.Commands.Stores.StoreOrders;
using Shop.Domain.Models.Stores;
using Shop.Domain.Models.Users;
using System;
using System.Linq;

namespace Shop.CommandHandlers
{
    /// <summary>
    /// 领域模型 事件处理
    /// </summary>
    [Component]
    public class StoreOrderCommandHandler:
        ICommandHandler<CreateStoreOrderCommand>,
        ICommandHandler<DeliverCommand>,
        ICommandHandler<ConfirmDeliverCommand>,
        ICommandHandler<ApplyRefundCommand>,
        ICommandHandler<ApplyReturnAndRefundCommand>,
        ICommandHandler<AgreeRefundCommand>,
        ICommandHandler<AgreeReturnCommand>
    {
        private readonly ILockService _lockService;

        /// <summary>
        /// IOC 构造函数注入
        /// </summary>
        /// <param name="lockService"></param>
        public StoreOrderCommandHandler(ILockService lockService)
        {
            _lockService = lockService;
        }

        #region handle Command

      
        
        public void Handle(ICommandContext context,CreateStoreOrderCommand command)
        {
            //从上下文中获取商家的地区信息
            var region = context.Get<Store>(command.StoreId).GetInfo().Region;
            //付款者钱包ID
            var walletId = context.Get<User>(command.UserId).GetWalletId();
            var userid = context.Get<Store>(command.StoreId).GetUserId();
            var storeownerwalletid = context.Get<User>(userid).GetWalletId();

            var storeOrder = new StoreOrder(
                    command.AggregateRootId,
                    walletId,
                    storeownerwalletid,
                    new Domain.Models.Stores.StoreOrders.StoreOrderInfo(
                        command.UserId,
                        command.OrderId,
                        command.StoreId,
                        region,
                        command.Number,
                        command.Remark),
                    new Domain.Models.Stores.StoreOrders.ExpressAddressInfo(
                        command.ExpressAddressInfo.Region,
                        command.ExpressAddressInfo.Address,
                        command.ExpressAddressInfo.Name,
                        command.ExpressAddressInfo.Mobile,
                        command.ExpressAddressInfo.Zip
                        ),
                    command.OrderGoodses.Select(x => new OrderGoodsInfo(
                        x.GoodsId,
                        x.SpecificationId,
                        walletId,
                        storeownerwalletid,
                        x.GoodsName,
                        x.GoodsPic,
                        x.SpecificationName,
                        x.Price,
                        x.OrigianlPrice,
                        x.Quantity,
                        x.Total,
                        x.StoreTotal,
                        DateTime.Now,
                        x.Surrender)
                    ).ToList()
                );
            //添加到上下文
            context.Add(storeOrder);
        }

        public void Handle(ICommandContext context, DeliverCommand command)
        {
            context.Get<StoreOrder>(command.AggregateRootId).Deliver(new Domain.Models.Stores.StoreOrders.ExpressInfo(
                command.ExpressName,
                command.ExpressNumber));
        }

        public void Handle(ICommandContext context, ApplyRefundCommand command)
        {
            context.Get<StoreOrder>(command.AggregateRootId).ApplyRefund(new Domain.Models.Stores.StoreOrders.RefoundApplyInfo(command.Reason,
                command.RefundAmount));
        }

        public void Handle(ICommandContext context, ApplyReturnAndRefundCommand command)
        {
            context.Get<StoreOrder>(command.AggregateRootId).ApplyReturnAndRefund(new Domain.Models.Stores.StoreOrders.RefoundApplyInfo(command.Reason,
                command.RefundAmount));
        }

        public void Handle(ICommandContext context, AgreeRefundCommand command)
        {
            context.Get<StoreOrder>(command.AggregateRootId).AgreeRefund();
        }

        public void Handle(ICommandContext context, AgreeReturnCommand command)
        {
            context.Get<StoreOrder>(command.AggregateRootId).AgreeReturn();
        }

        public void Handle(ICommandContext context, ConfirmDeliverCommand command)
        {
            context.Get<StoreOrder>(command.AggregateRootId).ConfirmExpress();
        }

        #endregion
    }
}
