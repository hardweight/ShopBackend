using System;
using System.Collections.Generic;
using System.Linq;
using ECommon.Components;
using Buy.Domain.Orders.Models;

namespace Buy.Domain.Orders
{
    [Component]
    public class DefaultPricingService : IPricingService
    {
        /// <summary>
        /// 订单小计计算
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="specificationQuantitys"></param>
        /// <returns></returns>
        public OrderTotal CalculateTotal(IEnumerable<SpecificationQuantity> specificationQuantitys)
        {
            var orderLines = new List<OrderLine>();
            foreach (var specificationQuantity in specificationQuantitys)
            {
                orderLines.Add(new OrderLine(specificationQuantity, Math.Round(specificationQuantity.Specification.UnitPrice * specificationQuantity.Quantity, 2)));
            }
            return new OrderTotal(orderLines.ToArray(), orderLines.Sum(x => x.LineTotal));
        }
    }
}
