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
        public string GoodsName { get; private set; }
        public string SpecificationName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal Total { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public decimal SurrenderPersent { get; private set; }

        public OrderGoodsInfo(
            Guid goodsId,
            Guid specificationId,
            string goodsName,
            string specificationName,
            decimal unitPrice,
            int quantity,
            decimal total,
            DateTime expirationDate,
            decimal surrenderPersent)
        {
            GoodsId = goodsId;
            SpecificationId = specificationId;
            GoodsName = goodsName;
            SpecificationName = specificationName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            Total = total;
            ExpirationDate = expirationDate;
            SurrenderPersent = surrenderPersent;
        }
    }
}
