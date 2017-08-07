using ECommon.Components;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Domain.Events.Goodses.Specifications;
using Shop.Domain.Models.PublishableExceptions;
using Shop.Messages.Goodses;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Messages.MessagePublisher
{
    /// <summary>
    /// 商品信息发布者 消息是跨聚合跟边界用的 不超过聚合跟边界，过程处理器可直接处理领域事件
    /// </summary>
    [Component]
    public class GoodsMessagePublisher:
        IMessageHandler<SpecificationReservedEvent>,
        IMessageHandler<SpecificationReservationCommittedEvent>,
        IMessageHandler<SpecificationReservationCancelledEvent>,
        IMessageHandler<SpecificationInsufficientException>
    {
        private readonly IMessagePublisher<IApplicationMessage> _messagePublisher;

        public GoodsMessagePublisher(IMessagePublisher<IApplicationMessage> messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        /// <summary>
        /// 处理商品预定事件 发出商品预定消息
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(SpecificationReservedEvent evnt)
        {
            return _messagePublisher.PublishAsync(new SpecificationReservedMessage
            (
                evnt.AggregateRootId,
                evnt.ReservationId,
                evnt.ReservationItems.Select(x => new SpecificationReservationItem (x.SpecificationId,x.Quantity )).ToList()
            ));
        }

        public Task<AsyncTaskResult> HandleAsync(SpecificationReservationCommittedEvent evnt)
        {
            return _messagePublisher.PublishAsync(new SpecificationReservationCommittedMessage
            (
                evnt.AggregateRootId,
                evnt.ReservationId
            ));
        }
        public Task<AsyncTaskResult> HandleAsync(SpecificationReservationCancelledEvent evnt)
        {
            return _messagePublisher.PublishAsync(new SpecificationReservationCancelledMessage
            (
                evnt.AggregateRootId,
                evnt.ReservationId
            ));
        }

        public Task<AsyncTaskResult> HandleAsync(SpecificationInsufficientException exception)
        {
            return _messagePublisher.PublishAsync(new SpecificationInsufficientMessage
            (
                exception.GoodsId,
                exception.ReservationId
            ));
        }
    }
}
