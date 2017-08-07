using ECommon.Components;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Domain.Events.Wallets;
using Shop.Messages.Messages.Users;
using System.Threading.Tasks;

namespace Shop.Messages.MessagePublisher
{
    /// <summary>
    /// /钱包信息发布者 消息是跨聚合跟边界用的 不超过聚合跟边界，过程处理器可直接处理领域事件
    /// </summary>
    [Component]
    public class WalletMessagePublisher:
        IMessageHandler<IncentiveUserBenevolenceEvent>
    {
        private readonly IMessagePublisher<IApplicationMessage> _messagePublisher;

        public WalletMessagePublisher(IMessagePublisher<IApplicationMessage> messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }

        /// <summary>
        /// 发布 激励用户善心 消息
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(IncentiveUserBenevolenceEvent evnt)
        {
            return _messagePublisher.PublishAsync(new IncentiveUserBenevolenceMessage
            (
                evnt.AggregateRootId,
                evnt.UserId,
                evnt.BenevolenceIndex,
                evnt.IncentiveValue,
                evnt.BenevolenceDeduct
            ));
        }
    }
}
