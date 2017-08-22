using Shop.Common.Enums;
using System;

namespace Shop.Api.Models.Request.StoreOrders
{
    public class StoreOrdersRequest
    {
        public Guid Id { get; set; }
        public StoreOrderStatus Status { get; set; }
    }
}