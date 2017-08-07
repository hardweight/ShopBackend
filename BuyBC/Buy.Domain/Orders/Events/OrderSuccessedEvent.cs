using System;

namespace Buy.Domain.Orders.Events
{
    [Serializable]
    public class OrderSuccessedEvent:OrderEvent
    {
        public OrderSuccessedEvent(){ }
    }
}
