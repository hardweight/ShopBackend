using System;
using ENode.Infrastructure;

namespace Shop.Messages.Store
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
        public Guid OrderGoodsId { get; set; }
    }
}