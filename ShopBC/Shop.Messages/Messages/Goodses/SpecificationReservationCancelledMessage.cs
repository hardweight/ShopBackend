using System;
using ENode.Infrastructure;

namespace Shop.Messages.Goodses
{
    /// <summary>
    /// 商品预定取消消息
    /// </summary>
    [Serializable]
    public class SpecificationReservationCancelledMessage : ApplicationMessage
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get;private set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid ReservationId { get;private set; }

        public SpecificationReservationCancelledMessage() { }
        public SpecificationReservationCancelledMessage(Guid goodsId,Guid reservationId)
        {
            GoodsId = goodsId;
            ReservationId = reservationId;
        }

    }
}