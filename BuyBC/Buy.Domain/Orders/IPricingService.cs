using Buy.Domain.Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buy.Domain.Orders
{
    /// <summary>
    /// 订单小计计算
    /// </summary>
    public interface IPricingService
    {
        OrderTotal CalculateTotal(IEnumerable<SpecificationQuantity> specifications);
    }
}
