using System;
using System.Collections.Generic;
using ENode.Infrastructure;

namespace Shop.Messages.Goodses
{
    /// <summary>
    /// 商品预定消息
    /// </summary>
    [Serializable]
    public class SpecificationReservedMessage : ApplicationMessage
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public Guid GoodsId { get;private set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public Guid ReservationId { get;private set; }
        /// <summary>
        /// 预定项目
        /// </summary>
        public IEnumerable<SpecificationReservationItem> ReservationItems { get; set; }


        public SpecificationReservedMessage() { }
        public SpecificationReservedMessage(Guid goodsId,Guid reservationId,IEnumerable<SpecificationReservationItem> reservationItems)
        {
            GoodsId = goodsId;
            ReservationId = reservationId;
            ReservationItems = reservationItems;
        }
    }
}