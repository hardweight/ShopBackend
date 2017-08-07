using Buy.Domain.Orders.Models;
using ENode.Eventing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buy.Domain.Orders.Events
{
    [Serializable]
    public class OrderEvent:DomainEvent<Guid>
    {
        public OrderTotal OrderTotal { get; private set; }

        public OrderEvent() { }
        public OrderEvent(OrderTotal orderTotal) {
            OrderTotal = orderTotal;
        }
    }
}
