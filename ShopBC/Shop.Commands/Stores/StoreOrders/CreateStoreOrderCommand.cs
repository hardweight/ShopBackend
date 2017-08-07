using ENode.Commanding;
using System;
using System.Collections.Generic;

namespace Shop.Commands.Stores.StoreOrders
{
    /// <summary>
    /// 创建商家订单   由用户订单拆分过来的订单  和用户订单相同订单号
    /// </summary>
    public class CreateStoreOrderCommand:Command<Guid>
    {
        public Guid StoreId { get; private set; }
        public Guid OrderId { get; private set; }
        public IList<OrderGoods> OrderGoodses { get; private set; }

        public CreateStoreOrderCommand() { }
        public CreateStoreOrderCommand(Guid id,Guid storeId,Guid orderId,IList<OrderGoods> orderGoodses):base(id)
        {
            StoreId = storeId;
            OrderId = orderId;
            OrderGoodses = orderGoodses;
        }
    }

    
}
