using System;

namespace Buy.Domain.Orders.Models
{
    /// <summary>
    /// 商品线
    /// </summary>
    [Serializable]
    public class OrderLine
    {
        public SpecificationQuantity SpecificationQuantity { get; private set; }
        public decimal LineTotal { get; private set; }

        public OrderLine() { }
        public OrderLine(SpecificationQuantity specificationQuantity, decimal lineTotal)
        {
            SpecificationQuantity = specificationQuantity;
            LineTotal = lineTotal;
        }
    }
}
