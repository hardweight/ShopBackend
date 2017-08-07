using System;

namespace Buy.Domain.Orders.Events
{
    [Serializable]
    public class OrderExpiredEvent: OrderEvent
    {
        public OrderExpiredEvent() { }
    }
}
