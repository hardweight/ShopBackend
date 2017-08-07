using System;
using System.Collections.Generic;

namespace Shop.Domain.Models.Goodses
{
    [Serializable]
    public class GoodsInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        /// <summary>
        /// 商品图片 多个规格建议每个规格一张照片，一个规格建议多方位图片
        /// </summary>
        public IList<string> Pics { get; private set; }
        /// <summary>
        /// 商品让利信息 目前是不管商品的规格信息固定让利信息
        /// </summary>
        public SurrenderInfo SurrenderInfo { get; private set; }
        public decimal Price { get; private set; }
        public decimal OriginalPrice { get; private set; }
        public int Stock { get; private set; }
        public int SellOut { get; private set; }
        public bool IsPayOnDelivery { get; private set; }
        public bool IsInvoice { get; private set; }
        public bool Is7SalesReturn { get; private set; }
        /// <summary>
        /// 排序编号 数字越大，排名越靠前,如果为空，默认排序方式为创建时间
        /// </summary>
        public int Sort { get; private set; }

        public GoodsInfo(string name,string description,IList<string> pics,decimal price,decimal originalPrice,int stock, SurrenderInfo surrenderInfo,int sellOut,bool isPayOnDelivery,bool isInvoice,bool is7SalesReturn,int sort)
        {
            Name = name;
            Description = description;
            Pics = pics;
            Price = price;
            OriginalPrice = originalPrice;
            Stock = stock;
            SurrenderInfo = surrenderInfo;
            SellOut = sellOut;
            IsPayOnDelivery = isPayOnDelivery;
            IsInvoice = isInvoice;
            Is7SalesReturn = is7SalesReturn;
            Sort = sort;
        }
    }

    
    
}
