using Shop.Common.Enums;
using Shop.ReadModel.StoreOrders.Dtos;
using System;
using System.Collections.Generic;

namespace Shop.ReadModel.StoreOrders
{
    public interface IStoreOrderQueryService
    {
        StoreOrderDetails FindOrder(Guid orderId);
        StoreOrder Find(Guid id);

        IEnumerable<StoreOrderWithInfo> StoreOrderList();

        IEnumerable<StoreOrder> StoreStoreOrders(Guid storeId);
        IEnumerable<StoreOrder> UserStoreOrders(Guid userId);

        IEnumerable<StoreOrderDetails> UserStoreOrderDetails(Guid userId);
        IEnumerable<StoreOrderDetails> UserStoreOrderDetails(Guid userId, StoreOrderStatus status);

        IEnumerable<StoreOrderDetails> StoreStoreOrderDetails(Guid storeId);
        IEnumerable<StoreOrderDetails> StoreStoreOrderDetails(Guid storeId,StoreOrderStatus status);
    }
}
