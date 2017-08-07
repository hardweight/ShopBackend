using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Carts
{
    public  class CartGoodsQuantityChangedEvent:DomainEvent<Guid>
    {
        public Guid CartGoodsId { get; private set; }
        public int FinalQuantity { get; private set; }

        public CartGoodsQuantityChangedEvent() { }
        public CartGoodsQuantityChangedEvent(Guid cartGoodsId,int finalQuantity)
        {
            CartGoodsId = cartGoodsId;
            FinalQuantity = finalQuantity;
        }
    }
}
