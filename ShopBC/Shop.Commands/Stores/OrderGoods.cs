using System;

namespace Shop.Commands.Stores
{
    /// <summary>
    /// 订单商品 命令端
    /// </summary>
    [Serializable]
    public class OrderGoods
    {
        public Guid GoodsId { get;private set; }
        public Guid SpecificationId { get; private set; }
        public string SpecificationName { get; private set; }
        public int Quantity { get; private set; }
        public decimal Total { get; private set; }
        public decimal SurrenderPersent { get; private set; }

        public OrderGoods(Guid goodsId,
            Guid specificationId,
            string specificationName,
            int quantity,
            decimal total,
            decimal surrenderPersent)
        {
            GoodsId = goodsId;
            SpecificationId = specificationId;
            SpecificationName = specificationName;
            Quantity = quantity;
            Total = total;
            SurrenderPersent = surrenderPersent;
        }
    }
}
