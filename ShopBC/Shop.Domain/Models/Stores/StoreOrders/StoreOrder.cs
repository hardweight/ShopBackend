using ENode.Domain;
using Shop.Domain.Events.Stores.StoreOrders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Domain.Models.Stores
{
    /// <summary>
    /// 商家订单 聚合跟
    /// </summary>
    public class StoreOrder:AggregateRoot<Guid>
    {
        private Guid _orderId;//订单号
        private Guid _storeId;//所属商家
        private string _region;//商家所在地区
        private IList<OrderGoodsInfo> _orderGoodses;//订单商品

        public StoreOrder(Guid id,Guid orderId,Guid storeId,string region,IList<OrderGoodsInfo> orderGoodses):base(id)
        {
            ApplyEvent(new StoreOrderCreatedEvent(orderId, storeId,region, orderGoodses));
        }

        /// <summary>
        /// 获取订单商品总额
        /// </summary>
        /// <returns></returns>
        public decimal GetTotal()
        {
            return _orderGoodses.Sum(x=>x.Total);
        }

        #region Handle
        private void Handle(StoreOrderCreatedEvent evnt)
        {
            _orderId = evnt.OrderId;
            _storeId = evnt.StoreId;
            _region = evnt.Region;
            _orderGoodses = evnt.OrderGoodses;
        }
        #endregion
    }
}
