using System;
using ENode.Infrastructure;

namespace Shop.Messages.Stores
{
    /// <summary>
    /// 商品服务结束消息 消息是跨聚合跟边界用的 不超过聚合跟边界，过程处理器可直接处理领域事件
    /// </summary>
    [Serializable]
    public class ServiceFinishedMessage : ApplicationMessage
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid OrderGoodsId { get;private set; }

        public ServiceFinishedMessage() { }
        public ServiceFinishedMessage(Guid orderGoodsId)
        {
            OrderGoodsId = orderGoodsId;
        }
    }
}