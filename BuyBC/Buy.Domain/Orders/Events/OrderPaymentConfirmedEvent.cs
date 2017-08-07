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
