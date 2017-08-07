using ENode.Eventing;
using Shop.Domain.Models.Carts.CartGoods;
using System;

namespace Shop.Domain.Events.Carts
{
    public class CartAddedGoodsEvent:DomainEvent<Guid>
    {
        public Guid CartGoodsId { get; private set; }
        public CartGoodsInfo Info { get; private set; }

        public CartAddedGoodsEvent() { }
        public CartAddedGoodsEvent(Guid cartGoodsId,CartGoodsInfo info)
        {
            CartGoodsId = cartGoodsId;
            Info = info;
        }
    }
}
