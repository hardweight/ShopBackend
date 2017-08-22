using Shop.ReadModel.Orders.Dtos;
using System;
using System.Collections.Generic;

namespace Shop.ReadModel.Orders
{
    public interface IOrderQueryService
    {
        Order FindOrder(Guid orderId);

        IEnumerable<OrderAlis> ExpiredUnPayOrders();
    }
}