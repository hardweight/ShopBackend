using Buy.Domain.Orders.Models;
using ENode.Eventing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buy.Domain.Orders.Events
{
    /// <summary>
    /// 订单某个商品预定确认事件
    /// </summary>
    [Serializable]
    public class OrderReservationOneConfirmedEvent:DomainEvent<Guid>
    {
        public bool IsReservationSuccess { get; private set; }
        public Guid GoodsId { get; private set; }
        public OrderReservationOneConfirmedEvent() { }
        public OrderReservationOneConfirmedEvent(Guid goodsId, bool isReservationSuccess)
        {
            GoodsId = goodsId;
            IsReservationSuccess = isReservationSuccess;
        }
    }
}
