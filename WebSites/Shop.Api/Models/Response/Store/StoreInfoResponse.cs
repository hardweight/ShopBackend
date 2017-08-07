using System;

namespace Shop.Api.Models.Response.Store
{
    public class StoreInfoResponse:BaseApiResponse
    {
        public StoreInfo StoreInfo { get; set; }
    }
    public class StoreInfo
    {
        public Guid Id { get; set; }
        public string AccessCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
    }
}