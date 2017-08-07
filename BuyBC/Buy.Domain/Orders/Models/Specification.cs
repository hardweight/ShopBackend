using System;

namespace Buy.Domain.Orders.Models
{
    /// <summary>
    /// 预定的商品规格
    /// </summary>
    [Serializable]
    public class Specification
    {
        public Guid SpecificationId { get; private set; }
        public Guid GoodsId { get; private set; }
        public Guid StoreId { get; private set; }
        public string SpecificationName { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal SurrenderPersent { get; private set; }

        public Specification() { }
        public Specification(Guid specificationId,Guid goodsId, Guid storeId,string specificationName, decimal unitPrice,decimal surrenderPersent)
        {
            SpecificationId = specificationId;
            GoodsId = goodsId;
            StoreId = storeId;
            SpecificationName = specificationName;
            UnitPrice = unitPrice;
            SurrenderPersent = surrenderPersent;
        }
    }
}
