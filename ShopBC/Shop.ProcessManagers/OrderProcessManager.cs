using ECommon.Components;
using ECommon.IO;
using ENode.Commanding;
using ENode.Infrastructure;
using Shop.Commands.Goodses.Specifications;
using Shop.Commands.Orders;
using Shop.Commands.Stores;
using Shop.Commands.Stores.StoreOrders;
using Shop.Common.Enums;
using Shop.Domain.Events.Orders;
using Shop.Domain.Models.Orders;
using Shop.Messages.Goodses;
using Shop.Messages.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xia.Common;
using Xia.Common.Extensions;

namespace Buy.ProcessManagers
{
    [Component]
    public class OrderProcessManager :
        IMessageHandler<OrderPlacedEvent>,                           //订单创建时发生(Order)
        IMessageHandler<SpecificationReservedMessage>,                  //预扣库存，成功时发生(Goods)
        IMessageHandler<SpecificationInsufficientMessage>,               //预扣库存，库存不足时发生(Goods)
        IMessageHandler<PaymentCompletedMessage>,               //支付成功时发生(Payment)
        IMessageHandler<PaymentRejectedMessage>,                //支付拒绝时发生(Payment)
        IMessageHandler<OrderPaymentConfirmedEvent>,                 //确认支付时发生(Order)
        IMessageHandler<SpecificationReservationCommittedMessage>,      //预扣库存提交时发生(Goods)
        IMessageHandler<SpecificationReservationCancelledMessage>,      //预扣库存取消时发生(Goods)
        IMessageHandler<OrderSuccessedEvent>,                        //订单处理成功时发生(Order)
        IMessageHandler<OrderExpiredEvent>                        //订单过期时(30分钟过期)发生(Order)
    {
        private ICommandService _commandService;

        public OrderProcessManager(ICommandService commandService)
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
            //通过商品ID 分组
            var goodsGroup = evnt.OrderTotal.Lines.GroupBy(x => x.SpecificationQuantity.Specification.GoodsId);
            foreach(var goods in goodsGroup)
            {
                //一个商品可以有多个规格的预定
                tasks.Add(_commandService.SendAsync(new MakeSpecificationReservationCommand(
                    goods.Key,
                     evnt.AggregateRootId,//订单ID
                                          //查找当前商品的所有规格
                    goods.Select(x => new SpecificationReservationItemInfo(
                            x.SpecificationQuantity.Specification.SpecificationId,
                            x.SpecificationQuantity.Quantity)).ToList()//所有规格和数量
                )));
            }
            //foreach (var goodsLine in evnt.OrderTotal.Lines)
            //{
            //    //一个商品可以有多个规格的预定
            //    tasks.Add( _commandService.SendAsync(new MakeSpecificationReservationCommand(
            //        goodsLine.SpecificationQuantity.Specification.GoodsId,
            //         evnt.AggregateRootId,//订单ID
            //         //查找当前商品的所有规格
            //        evnt.OrderTotal.Lines.Where(x=>x.SpecificationQuantity.Specification.GoodsId==goodsLine.SpecificationQuantity.Specification.GoodsId).Select(x => new SpecificationReservationItemInfo (
            //            x.SpecificationQuantity.Specification.SpecificationId,
            //            x.SpecificationQuantity.Quantity )).ToList()//所有规格和数量
            //    )));
            //}
            //执行所以的任务  
            Task.WaitAll(tasks.ToArray());
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
            //通过商品ID 分组
            var goodsGroup = evnt.OrderTotal.Lines.GroupBy(x => x.SpecificationQuantity.Specification.GoodsId);
            foreach (var goods in goodsGroup)
            {
                //每个商品都发送确认预定信息
                if (evnt.OrderStatus == OrderStatus.PaymentSuccess)
                {
                    tasks.Add(_commandService.SendAsync(new CommitSpecificationReservationCommand(
                        goods.Key, 
                        evnt.AggregateRootId)));
                }
                else if (evnt.OrderStatus == OrderStatus.PaymentRejected)
                {
                    tasks.Add(_commandService.SendAsync(new CancelSpecificationReservationCommand(
                        goods.Key,
                        evnt.AggregateRootId)));
                }
            }
            //foreach (var goodsLine in evnt.OrderTotal.Lines)
            //{ 
            //    //每个商品都发送确认预定信息
            //    if (evnt.OrderStatus == OrderStatus.PaymentSuccess)
            //    {
            //        tasks.Add( _commandService.SendAsync(new CommitSpecificationReservationCommand(goodsLine.SpecificationQuantity.Specification.GoodsId, evnt.AggregateRootId)));
            //    }
            //    else if (evnt.OrderStatus == OrderStatus.PaymentRejected)
            //    {
            //        tasks.Add( _commandService.SendAsync(new CancelSpecificationReservationCommand(goodsLine.SpecificationQuantity.Specification.GoodsId, evnt.AggregateRootId)));
            //    }
            //}
            Task.WaitAll(tasks.ToArray());
            //Task.WhenAll(tasks).ConfigureAwait(false);
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
        /// 订单付款成功  分单到商家
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
                tasks.Add(_commandService.SendAsync(new CreateStoreOrderCommand(
                    GuidUtil.NewSequentialId(),
                    evnt.UserId,
                    storeId,
                    evnt.AggregateRootId,
                    DateTime.Now.ToSerialNumber(),
                    "",//订单备注
                    new Shop.Commands.Stores.StoreOrders.ExpressAddressInfo(
                        evnt.ExpressAddressInfo.Region,
                        evnt.ExpressAddressInfo.Address,
                        evnt.ExpressAddressInfo.Name,
                        evnt.ExpressAddressInfo.Mobile,
                        evnt.ExpressAddressInfo.Zip),
                    goods.Select(x=>new OrderGoods (
                        x.SpecificationQuantity.Specification.GoodsId,
                        x.SpecificationQuantity.Specification.SpecificationId,
                        x.SpecificationQuantity.Specification.GoodsName,
                        x.SpecificationQuantity.Specification.GoodsPic,
                        x.SpecificationQuantity.Specification.SpecificationName,
                        x.SpecificationQuantity.Specification.Price,
                        x.SpecificationQuantity.Specification.OriginalPrice,
                        x.SpecificationQuantity.Quantity,
                        x.LineTotal,
                        x.StoreLineTotal,
                        x.SpecificationQuantity.Specification.Benevolence
                    )).ToList())));
            }
            Task.WaitAll(tasks.ToArray());
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
                tasks.Add( _commandService.SendAsync(
                    new CancelSpecificationReservationCommand(
                        goodsLine.SpecificationQuantity.Specification.GoodsId, 
                        evnt.AggregateRootId)));
            }
            Task.WaitAll(tasks.ToArray());
            return Task.FromResult(AsyncTaskResult.Success);
        }

        
    }
}
