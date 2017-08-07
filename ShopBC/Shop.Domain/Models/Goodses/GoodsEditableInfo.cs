using System;
using System.Collections.Generic;

namespace Shop.Domain.Models.Goodses
{
    [Serializable]
    public class GoodsEditableInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IList<string> Pics { get; private set; }
        public decimal Price { get; private set; }
        public decimal OriginalPrice { get; private set; }
        public int Stock { get; private set; }
        public decimal SurrenderPersent { get; private set; }
        public int SellOut { get; private set; }
        public bool IsPayOnDelivery { get; private set; }
        public bool IsInvoice { get; private set; }
        public bool Is7SalesReturn { get; private set; }
        /// <summary>
        /// 排序编号 数字越大，排名越靠前,如果为空，默认排序方式为创建时间
        /// </summary>
        public int Sort { get; private set; }

        public GoodsEditableInfo(string name,string description,IList<string> pics,decimal price,decimal originalPrice,int stock,decimal surrenderPersent,int sellOut,bool isPayOnDelivery,bool isInvoice,bool is7SalesReturn,int sort)
        {
            Name = name;
            Description = description;
            Pics = pics;
            Price = price;
            OriginalPrice = originalPrice;
            Stock = stock;
            SurrenderPersent = surrenderPersent;
            SellOut = sellOut;
            IsPayOnDelivery = isPayOnDelivery;
            IsInvoice = isInvoice;
            Is7SalesReturn = is7SalesReturn;
            Sort = sort;
        }
    }
}
