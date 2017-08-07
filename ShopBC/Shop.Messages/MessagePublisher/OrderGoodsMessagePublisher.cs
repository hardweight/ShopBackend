using ECommon.Components;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Domain.Events.Stores.StoreOrders.GoodsServices;
using Shop.Messages.Stores;
using System.Threading.Tasks;

namespace Shop.Messages.MessagePublisher
{
    /// <summary>
    /// 商品信息发布者  消息是跨聚合跟边界用的 不超过聚合跟边界，过程处理器可直接处理领域事件
    /// </summary>
    [Component]
    public class OrderGoodsMessagePublisher:
        IMessageHandler<ServiceExpiredEvent>,
        IMessageHandler<ServiceFinishedEvent>
    {
        private readonly IMessagePublisher<IApplicationMessage> _messagePublisher;

        public OrderGoodsMessagePublisher(IMessagePublisher<IApplicationMessage> messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        public Task<AsyncTaskResult> HandleAsync(ServiceExpiredEvent evnt)
        {
            return _messagePublisher.PublishAsync(new ServiceExpiredMessage
            (
                evnt.AggregateRootId
            ));
        }
        public Task<AsyncTaskResult> HandleAsync(ServiceFinishedEvent evnt)
        {
            return _messagePublisher.PublishAsync(new ServiceFinishedMessage
            (
                evnt.AggregateRootId
            ));
        }

        
    }
}
