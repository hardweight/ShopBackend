using System;

namespace Shop.Api.Models.Request.StoreOrders
{
    public class DeliverRequest
    {
        public Guid Id { get; set; }
        public ExpressInfo ExpressInfo { get; set; }
    }

    public class ExpressInfo
    {
        public string ExpressName { get; set; }
        public string ExpressNumber { get; set; }
    }
}