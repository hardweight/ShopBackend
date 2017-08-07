using System;
using ENode.Infrastructure;

namespace Shop.Messages.Goodses
{
    /// <summary>
    /// 商品预定成功消息
    /// </summary>
    [Serializable]
    public class SpecificationReservationCommittedMessage : ApplicationMessage
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get;private set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid ReservationId { get; private set; }


        public SpecificationReservationCommittedMessage() { }
        public SpecificationReservationCommittedMessage(Guid goodsId,Guid reservationId)
        {
            GoodsId = goodsId;
            ReservationId = reservationId;
        }
    }
}