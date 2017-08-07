using Buy.Commands.Orders;
using Buy.Domain.Orders.Events;
using Buy.Domain.Orders.Models;
using ECommon.Components;
using ECommon.IO;
using ENode.Commanding;
using ENode.Infrastructure;
using Payments.Messages;
using Shop.Commands.Goodses.Specifications;
using Shop.Commands.Stores;
using Shop.Commands.Stores.StoreOrders;
using Shop.Messages.Goodses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buy.ProcessManagers
{
    [Component]
    public class BuyProcessManager :
        IMessageHandler<OrderPlacedEvent>,                           //订单创建时发生(Order)
        IMessageHandler<SpecificationReservedMessage>,                  //预扣库存，成功时发生(Goods)
        IMessageHandler<SpecificationInsufficientMessage>,               //预扣库存，库存不足时发生(Goods)
        IMessageHandler<PaymentCompletedMessage>,               //支付成功时发生(Payment)
        IMessageHandler<PaymentRejectedMessage>,                //支付拒绝时发生(Payment)
        IMessageHandler<OrderPaymentConfirmedEvent>,                 //确认支付时发生(Order)
        IMessageHandler<SpecificationReservationCommittedMessage>,      //预扣库存提交时发生(Goods)
        IMessageHandler<SpecificationReservationCancelledMessage>,      //预扣库存取消时发生(Goods)
        IMessageHandler<OrderSuccessedEvent>,                        //订单处理成功时发生(Order)
        IMessageHandler<OrderExpiredEvent>                        //订单过期时(15分钟过期)发生(Order)
    {
        private ICommandService _commandService;

        public BuyProcessManager(ICommandService commandService)
        {
            _commandService = commandService;
        }

        /// <summary>
        /// 订单预定  发起到所有商品规格的预定命令
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public  Task<AsyncTaskResult> HandleAsync(OrderPlacedEvent evnt)
        {
            var tasks = new List<Task>();
            foreach (var goodsLine in evnt.OrderTotal.Lines)
            {
                //这里涉及多个商品的预定
                tasks.Add( _commandService.SendAsync(new MakeSpecificationReservationCommand(
                    goodsLine.SpecificationQuantity.Specification.GoodsId,
                     evnt.AggregateRootId,//订单ID
                    evnt.OrderTotal.Lines.Select(x => new SpecificationReservationItemInfo (
                        x.SpecificationQuantity.Specification.SpecificationId,
                        x.SpecificationQuantity.Quantity )).ToList()//所有规格和数量
                )));
            }
            //执行所以的任务  
            Task.WhenAll(tasks).ConfigureAwait(false);
            return Task.FromResult(AsyncTaskResult.Success);
        }
        
        /// <summary>
        /// 某个商品发来的预定成功消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(SpecificationReservedMessage message)
        {
            //发送单个商品预定成功指令给订单 订单处理是否全部预定成功
            return _commandService.SendAsync(new ConfirmOneReservationCommand( message.ReservationId,message.GoodsId, true));
        }
        /// <summary>
        /// 某个商品发来的预定库存不足的消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(SpecificationInsufficientMessage message)
        {
            return _commandService.SendAsync(new ConfirmOneReservationCommand(message.ReservationId,message.GoodsId, false));
        }

        public Task<AsyncTaskResult> HandleAsync(PaymentCompletedMessage message)
        {
            return _commandService.SendAsync(new ConfirmPaymentCommand(message.OrderId, true));
        }
        public Task<AsyncTaskResult> HandleAsync(PaymentRejectedMessage message)
        {
            return _commandService.SendAsync(new ConfirmPaymentCommand(message.OrderId, false));
        }

        /// <summary>
        /// 订单付款确认信息 提交商品规格预定
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(OrderPaymentConfirmedEvent evnt)
        {
            var tasks = new List<Task>();
            foreach(var goodsLine in evnt.OrderTotal.Lines)
            { 
                //每个商品都发送确认预定信息
                if (evnt.OrderStatus == OrderStatus.PaymentSuccess)
                {
                    tasks.Add( _commandService.SendAsync(new CommitSpecificationReservationCommand(goodsLine.SpecificationQuantity.Specification.GoodsId, evnt.AggregateRootId)));
                }
                else if (evnt.OrderStatus == OrderStatus.PaymentRejected)
                {
                    tasks.Add( _commandService.SendAsync(new CancelSpecificationReservationCommand(goodsLine.SpecificationQuantity.Specification.GoodsId, evnt.AggregateRootId)));
                }
            }
            Task.WhenAll(tasks).ConfigureAwait(false);
            return Task.FromResult(AsyncTaskResult.Success);
        }

        /// <summary>
        /// 商品规格预定确认返回 因为已经预定，所有确认一定会成功 每个商品返回都会发送一个成功命令
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(SpecificationReservationCommittedMessage message)
        {
            return _commandService.SendAsync(new MarkAsSuccessCommand(message.ReservationId,message.GoodsId));
        }
        /// <summary>
        /// 每个商品都会发送一个取消命令
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(SpecificationReservationCancelledMessage message)
        {
            return _commandService.SendAsync(new CloseOrderCommand(message.ReservationId,message.GoodsId));
        }

        /// <summary>
        /// 订单付款成功  目前工作的很好， 但指派给商家这个工作是否要交给shopBC处理？
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(OrderSuccessedEvent evnt)
        {
            var tasks = new List<Task>();
            //商品按商家ID 分组
            var groupInfo = evnt.OrderTotal.Lines.GroupBy(m => m.SpecificationQuantity.Specification.StoreId).ToList();
            foreach (var item in groupInfo)
            {
                //遍历每个商家
                var storeId = item.Key;
                var goods = item.ToList<OrderLine>();
                //提交订单到各个商家 怎样获取商家的地区？
                tasks.Add(_commandService.SendAsync(new CreateStoreOrderCommand(
                    Guid.NewGuid(),
                    storeId,
                    evnt.AggregateRootId,
                    goods.Select(x=>new OrderGoods (
                        x.SpecificationQuantity.Specification.GoodsId,
                        x.SpecificationQuantity.Specification.SpecificationId,
                        x.SpecificationQuantity.Specification.SpecificationName,
                        x.SpecificationQuantity.Quantity,
                        x.LineTotal,
                        x.SpecificationQuantity.Specification.SurrenderPersent
                    )).ToList())));
            }

            Task.WhenAll(tasks).ConfigureAwait(false);
            return Task.FromResult(AsyncTaskResult.Success);
        }

        /// <summary>
        /// 订单过期
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(OrderExpiredEvent evnt)
        {
            var tasks = new List<Task>();
            foreach (var goodsLine in evnt.OrderTotal.Lines)
            {
                //发送给所有商品 取消预定
                tasks.Add( _commandService.SendAsync(new CancelSpecificationReservationCommand(goodsLine.SpecificationQuantity.Specification.GoodsId, evnt.AggregateRootId)));
            }
            Task.WhenAll(tasks).ConfigureAwait(false);
            return Task.FromResult(AsyncTaskResult.Success);
        }

        
    }
}
