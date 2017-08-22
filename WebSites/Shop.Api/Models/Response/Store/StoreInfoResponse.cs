using Shop.Api.Models.Response.StoreOrders;
using System;
using System.Collections.Generic;

namespace Shop.Api.Models.Response.Store
{
    public class StoreInfoResponse:BaseApiResponse
    {
        public StoreInfo StoreInfo { get; set; }
        public StatisticsInfo StatisticsInfo { get; set; }
        /// <summary>
        /// 新订单
        /// </summary>
        public IList<StoreOrder> StoreOrders { get; set; }
    }
    public class StoreInfo
    {
        public Guid Id { get; set; }
        public string AccessCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class StatisticsInfo
    {
        public decimal TodaySale { get; set; }
        public decimal TotalSale { get; set; }
        public int TodayOrder { get; set; }
        public int TotalOrder { get; set; }
        public int OnSaleGoodsCount { get; set; }
    }
}