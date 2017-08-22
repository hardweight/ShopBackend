using Shop.Domain.Models.Orders;
using System;

namespace Shop.Domain.Events.Orders
{
    [Serializable]
    public class OrderSuccessedEvent:OrderEvent
    {
        public Guid UserId { get; set; }
        public ExpressAddressInfo ExpressAddressInfo { get; set; }
        public OrderSuccessedEvent() { }
        public OrderSuccessedEvent(Guid userId,OrderTotal orderTotal,ExpressAddressInfo expressAddressInfo):base(orderTotal)
        {
            UserId = userId;
            ExpressAddressInfo = expressAddressInfo;
        }
    }
}
