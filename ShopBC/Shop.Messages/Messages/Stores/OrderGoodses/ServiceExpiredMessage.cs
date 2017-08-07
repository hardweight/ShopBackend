using System;
using ENode.Infrastructure;

namespace Shop.Messages.Stores
{
    /// <summary>
    /// 商品服务期过期消息
    /// </summary>
    [Serializable]
    public class ServiceExpiredMessage : ApplicationMessage
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid OrderGoodsId { get; private set; }

        public ServiceExpiredMessage() { }
        public ServiceExpiredMessage(Guid orderGoodsId)
        {
            OrderGoodsId = orderGoodsId;
        }
    }
}