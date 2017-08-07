using System;
using ENode.Commanding;
using System.Collections.Generic;

namespace Shop.Commands.Goodses
{
    public class CreateGoodsCommand: Command<Guid>
    {
        public Guid StoreId { get;private set; }
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
        public int Sort { get; private set; }

        public CreateGoodsCommand() { }
        public CreateGoodsCommand(Guid id,
            Guid storeId,
            string name,
            string description,
            IList<string> pics,
            decimal price,
            decimal orginalPrice,
            int stock,
            decimal surrenderPersent,
            int sellOut,
            bool isPayOnDelivery,
            bool isInvoice,
            bool is7SalesReturn,
            int sort):base(id)
        {
            StoreId = storeId;
            Name = name;
            Description = description;
            Pics = pics;
            Price = price;
            OriginalPrice = orginalPrice;
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
