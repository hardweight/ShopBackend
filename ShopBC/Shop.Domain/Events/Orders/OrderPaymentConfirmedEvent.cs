using Shop.Common.Enums;
using Shop.Domain.Models.Orders;
using System;

namespace Shop.Domain.Events.Orders
{
    /// <summary>
    /// 订单支付确认事件
    /// </summary>
    [Serializable]
    public class OrderPaymentConfirmedEvent : OrderEvent
    {
        public OrderStatus OrderStatus { get; private set; }

        public OrderPaymentConfirmedEvent() { }
        public OrderPaymentConfirmedEvent(OrderTotal orderTotal, OrderStatus orderStatus):base(orderTotal)
        {
            OrderStatus = orderStatus;
        }
    }
}
