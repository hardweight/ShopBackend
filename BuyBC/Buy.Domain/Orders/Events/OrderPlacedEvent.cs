using System;
using System.Collections.Generic;
using ENode.Eventing;
using Buy.Domain.Orders.Models;

namespace Buy.Domain.Orders.Events
{
    [Serializable]
    public class OrderPlacedEvent : OrderEvent
    {
        public Guid UserId { get; private set; }
        public DateTime ReservationExpirationDate { get; private set; }

        public OrderPlacedEvent() { }
        public OrderPlacedEvent(Guid userId,OrderTotal orderTotal, DateTime reservationExpirationDate):base(orderTotal)
        {
            UserId = userId;
            ReservationExpirationDate = reservationExpirationDate;
        }
    }
}
