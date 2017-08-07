using System;

namespace Buy.Domain.Orders.Events
{
    [Serializable]
    public class OrderClosedEvent : OrderEvent
    {
        public OrderClosedEvent() { }
    }
}
