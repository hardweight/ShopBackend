using System;
using System.Collections.Generic;

namespace Shop.Domain.Models.Goodses
{
    [Serializable]
    public class GoodsInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IList<string> Pics { get; private set; }
        /// <summary>
        /// 商品让利信息 倍数
        /// </summary>
        public decimal Surrender { get; private set; }
        /// <summary>
        /// 平台售价
        /// </summary>
        public decimal Price { get; private set; }
        /// <summary>
        /// 供货价
        /// </summary>
        public decimal OriginalPrice { get; private set; }
        public int Stock { get; private set; }
        public int SellOut { get; private set; }
        public bool IsPayOnDelivery { get; private set; }
        public bool IsInvoice { get; private set; }
        public bool Is7SalesReturn { get; private set; }
        public int Sort { get; private set; }

        public GoodsInfo(string name,string description,IList<string> pics,decimal price,decimal originalPrice,int stock, decimal surrender, int sellOut,bool isPayOnDelivery,bool isInvoice,bool is7SalesReturn,int sort)
        {
            Name = name;
            Description = description;
            Pics = pics;
            Price = price;
            OriginalPrice = originalPrice;
            Stock = stock;
            Surrender = surrender;
            SellOut = sellOut;
            IsPayOnDelivery = isPayOnDelivery;
            IsInvoice = isInvoice;
            Is7SalesReturn = is7SalesReturn;
            Sort = sort;
        }
    }

    
    
}
