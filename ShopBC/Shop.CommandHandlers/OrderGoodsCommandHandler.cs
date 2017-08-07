using ECommon.Components;
using ENode.Commanding;
using ENode.Infrastructure;
using Shop.Commands.Stores.StoreOrders.OrderGoodses;
using Shop.Domain.Models.Stores;
using Shop.Domain.Models.Stores.StoreOrders.GoodsServices;
using System;

namespace Shop.CommandHandlers
{
    /// <summary>
    /// 领域模型 事件处理
    /// </summary>
    [Component]
    public class OrderGoodsCommandHandler:
        ICommandHandler<CreateOrderGoodsCommand>,
        ICommandHandler<ApplyServicesCommand>,
        ICommandHandler<AgreeServiceCommand>,
        ICommandHandler<AddServicesExpressInfoCommand>,
        ICommandHandler<AgreeRefundCommand>,
        ICommandHandler<DisAgreeRefundCommand>,
        ICommandHandler<ServiceFinishCommand>,
        ICommandHandler<ToDoorServiceFinishCommand>
    {
        private readonly ILockService _lockService;

        /// <summary>
        /// IOC 构造函数注入
        /// </summary>
        /// <param name="lockService"></param>
        public OrderGoodsCommandHandler(ILockService lockService)
        {
            _lockService = lockService;
        }

        #region handle Command
        public void Handle(ICommandContext context, CreateOrderGoodsCommand command)
        {
            var orderGoods = new OrderGoods(
                    command.AggregateRootId,
                    command.OrderId,
                    new OrderGoodsInfo(
                        command.OrderGoods.GoodsId,
                        command.OrderGoods.SpecificationId,
                        command.OrderGoods.SpecificationName,
                        command.OrderGoods.SpecificationName,
                        0,
                        command.OrderGoods.Quantity,
                        command.OrderGoods.Total,
                        DateTime.Now,
                        command.OrderGoods.SurrenderPersent)
                );
            //添加到上下文
            context.Add(orderGoods);
        }
        public void Handle(ICommandContext context, ApplyServicesCommand command)
        {
            var goodsServicesType = GoodsServiceType.Refund;
            switch(command.Info.ServiceType)
            {
                case ServiceType.Refund:
                    break;
                case ServiceType.SalesReturn:
                    goodsServicesType = GoodsServiceType.SalesReturn;
                    break;
                case ServiceType.Service:
                    goodsServicesType = GoodsServiceType.Service;
                    break;
                case ServiceType.ToDoorService:
                    goodsServicesType = GoodsServiceType.ToDoorService;
                    break;
                case ServiceType.Change:
                    goodsServicesType =GoodsServiceType.Change;
                    break;
            }
            context.Get<OrderGoods>(command.AggregateRootId).ApplyServices(new Domain.Models.Stores.StoreOrders.GoodsServices.ServiceApplyInfo(
                command.Info.ServiceNumber,
                goodsServicesType,
                command.Info.Quantity,
                command.Info.Reason,
                command.Info.Reason));
        }
        public void Handle(ICommandContext context, AgreeServiceCommand command)
        {
            context.Get<OrderGoods>(command.AggregateRootId).AgreeService(command.ServiceNumber);
        }
        public void Handle(ICommandContext context, AddServicesExpressInfoCommand command)
        {
            context.Get<OrderGoods>(command.AggregateRootId).AddServicesExpressInfo(new Domain.Models.Stores.StoreOrders.GoodsServices.ServiceExpressInfo(
                command.Info.ServiceNumber,
                command.Info.ExpressName,
                command.Info.ExpressNumber
            ));
        }
        public void Handle(ICommandContext context, AgreeRefundCommand command)
        {
            context.Get<OrderGoods>(command.AggregateRootId).AgreeRefund(command.ServiceNumber);
        }
        public void Handle(ICommandContext context, DisAgreeRefundCommand command)
        {
            context.Get<OrderGoods>(command.AggregateRootId).DisAgreeRefund(command.ServiceNumber);
        }
        public void Handle(ICommandContext context, ServiceFinishCommand command)
        {
            context.Get<OrderGoods>(command.AggregateRootId).ServiceFinish(new Domain.Models.Stores.StoreOrders.GoodsServices.ServiceFinishExpressInfo(
                command.Info.ExpressName,
                command.Info.ExpressNumber
            ));
        }
        public void Handle(ICommandContext context, ToDoorServiceFinishCommand command)
        {
            context.Get<OrderGoods>(command.AggregateRootId).ServiceFinish(command.ServiceNumber);
        }
        #endregion
    }
}
