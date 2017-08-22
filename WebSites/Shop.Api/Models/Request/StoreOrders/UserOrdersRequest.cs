using Shop.Common.Enums;

namespace Shop.Api.Models.Request.StoreOrders
{
    public class UserOrdersRequest
    {
        public StoreOrderStatus Status { get; set; }
    }
}