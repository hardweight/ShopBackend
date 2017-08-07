using System;

namespace Buy.Commands.Orders
{
    [Serializable]
    public class SpecificationiInfo
    {
        public Guid SpecificationId { get; private set; }
        public Guid GoodsId { get;private set; }
        public Guid StoreId { get; private set; }
        public string Name { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal SurrenderPersent { get; private set; }

        public SpecificationiInfo(Guid specificationId,Guid goodsId,Guid storeId,string name,decimal unitPrice,int quantity,decimal surrenderPersent)
        {
            SpecificationId = specificationId;
            GoodsId = goodsId;
            StoreId = storeId;
            Name = name;
            UnitPrice = unitPrice;
            Quantity = quantity;
            SurrenderPersent = surrenderPersent;
        }
    }
}
