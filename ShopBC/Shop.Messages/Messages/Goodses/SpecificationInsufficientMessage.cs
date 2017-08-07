using System;
using ENode.Infrastructure;

namespace Shop.Messages.Goodses
{
    /// <summary>
    /// 预定商品不足消息
    /// </summary>
    [Serializable]
    public class SpecificationInsufficientMessage : ApplicationMessage
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get;private set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid ReservationId { get;private set; }

        public SpecificationInsufficientMessage() { }
        public SpecificationInsufficientMessage(Guid goodsId,Guid reservationId)
        {
            GoodsId = goodsId;
            ReservationId = reservationId;
        }
    }
}