namespace Shop.Domain.Models.Stores
{
    /// <summary>
    /// 店铺统计信息
    /// </summary>
    public class StoreStatisticInfo
    {
        /// <summary>
        /// 今日销售额
        /// </summary>
        public decimal TodaySale { get; private set; }
        /// <summary>
        /// 累计销售额
        /// </summary>
        public decimal TotalSale { get; private set; }
        /// <summary>
        /// 在售商品量
        /// </summary>
        public int OnSaleGoodsCount { get; private set; }

        public StoreStatisticInfo(decimal todaySale,decimal totalSale,int onSaleGoodsCount)
        {
            TodaySale = todaySale;
            TotalSale = totalSale;
            OnSaleGoodsCount = onSaleGoodsCount;
        }
    }
}
