using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Shop.Common;
using ECommon.Components;
using ECommon.Dapper;
using Buy.ReadModel.Dtos;

namespace Buy.ReadModel
{
    [Component]
    public class OrderQueryService : IOrderQueryService
    {
        public Order FindOrder(Guid orderId)
        {
            using (var connection = GetConnection())
            {
                var order = connection.QueryList<Order>(new { OrderId = orderId }, ConfigSettings.OrderTable).FirstOrDefault();
                if (order != null)
                {
                    order.SetLines(connection.QueryList<OrderLine>(new { OrderId = orderId }, ConfigSettings.OrderLineTable).ToList());
                    return order;
                }
                return null;
            }
        }
        

        private IDbConnection GetConnection()
        {
            return new SqlConnection(ConfigSettings.ConnectionString);
        }
    }
}