using System;

namespace Shop.Domain.Models.Stores
{
    /// <summary>
    /// 订单商品信息
    /// </summary>
    public class OrderGoodsInfo
    {
        public Guid GoodsId { get;private set; }
        public Guid SpecificationId { get; private set; }
        public Guid WalletId { get; private set; }
        public Guid StoreOwnerWalletId { get;private set; }
        public string GoodsName { get; private set; }
        public string GoodsPic { get; private set; }
        public string SpecificationName { get; private set; }
        public decimal Price { get; private set; }
        public decimal OriginalPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal Total { get; private set; }
        public decimal StoreTotal { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public decimal Benevolence { get; private set; }

        public OrderGoodsInfo(
            Guid goodsId,
            Guid specificationId,
            Guid walletId,
            Guid storeOwnerWalletId,
            string goodsName,
            string goodsPic,
            string specificationName,
            decimal price,
            decimal originalPrice,
            int quantity,
            decimal total,
            decimal storeTotal,
            DateTime expirationDate,
            decimal benevolence)
        {
            GoodsId = goodsId;
            SpecificationId = specificationId;
            WalletId = walletId;
            StoreOwnerWalletId = storeOwnerWalletId;
            GoodsName = goodsName;
            GoodsPic = goodsPic;
            SpecificationName = specificationName;
            Price = price;
            OriginalPrice = originalPrice;
            Quantity = quantity;
            Total = total;
            StoreTotal = storeTotal;
            ExpirationDate = expirationDate;
            Benevolence = benevolence;
        }
    }
}
