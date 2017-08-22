using ECommon.Components;
using ECommon.Dapper;
using Shop.Common;
using Shop.ReadModel.StoreOrders.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Text;
using System.Threading.Tasks;
using Shop.Common.Enums;

namespace Shop.ReadModel.StoreOrders
{
    [Component]
    public class StoreOrderQueryService:BaseQueryService,IStoreOrderQueryService
    {
        public StoreOrderDetails FindOrder(Guid orderId)
        {
            using (var connection = GetConnection())
            {
                var order = connection.QueryList<StoreOrderDetails>(new { OrderId = orderId }, ConfigSettings.OrderTable).FirstOrDefault();
                if (order != null)
                {
                    order.StoreOrderGoodses = connection.QueryList<StoreOrderGoods>(new { OrderId = orderId }, ConfigSettings.OrderGoodsTable).ToList();
                    return order;
                }
                return null;
            }
        }

        public IEnumerable<StoreOrder> StoreStoreOrders(Guid storeId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<StoreOrder>(new { StoreId = storeId }, ConfigSettings.StoreOrderTable);
            }
        }
        public IEnumerable<StoreOrder> UserStoreOrders(Guid userId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<StoreOrder>(new { UserId = userId }, ConfigSettings.StoreOrderTable);
            }
        }

        public IEnumerable<StoreOrderDetails> UserStoreOrderDetails(Guid userId)
        {
            using (var connection = GetConnection())
            {
                var storeorders= connection.QueryList<StoreOrderDetails>(new { UserId = userId }, ConfigSettings.StoreOrderTable);
                foreach(var storeorder in storeorders)
                {
                    storeorder.StoreOrderGoodses= connection.QueryList<StoreOrderGoods>(new { OrderId = storeorder.Id }, ConfigSettings.OrderGoodsTable).ToList();
                }
                return storeorders;
            }
        }
        public IEnumerable<StoreOrderDetails> StoreStoreOrderDetails(Guid storeId)
        {
            using (var connection = GetConnection())
            {
                var storeorders = connection.QueryList<StoreOrderDetails>(new { StoreId = storeId }, ConfigSettings.StoreOrderTable);
                foreach (var storeorder in storeorders)
                {
                    storeorder.StoreOrderGoodses = connection.QueryList<StoreOrderGoods>(new { OrderId = storeorder.Id }, ConfigSettings.OrderGoodsTable).ToList();
                }
                return storeorders;
            }
        }

        public IEnumerable<StoreOrderDetails> StoreStoreOrderDetails(Guid storeId,StoreOrderStatus status)
        {
            using (var connection = GetConnection())
            {
                var storeorders = connection.QueryList<StoreOrderDetails>(new { StoreId = storeId,Status=status }, ConfigSettings.StoreOrderTable);
                foreach (var storeorder in storeorders)
                {
                    storeorder.StoreOrderGoodses = connection.QueryList<StoreOrderGoods>(new { OrderId = storeorder.Id }, ConfigSettings.OrderGoodsTable).ToList();
                }
                return storeorders;
            }
        }
    }
}
