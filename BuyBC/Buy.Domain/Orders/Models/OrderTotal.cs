using System;

namespace Buy.Domain.Orders.Models
{
    /// <summary>
    /// 订单总额 包含多个商品线
    /// </summary>
    [Serializable]
    public class OrderTotal
    {
        public OrderLine[] Lines { get; private set; }
        public decimal Total { get; private set; }

        public OrderTotal() { }
        public OrderTotal(OrderLine[] lines, decimal total)
        {
            Lines = lines;
            Total = total;
        }
    }
}
