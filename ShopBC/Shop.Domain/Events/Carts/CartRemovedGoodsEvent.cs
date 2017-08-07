﻿using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Carts
{
    public class CartRemovedGoodsEvent:DomainEvent<Guid>
    {
        public Guid CartGoodsId { get; private set; }

        public CartRemovedGoodsEvent() { }
        public CartRemovedGoodsEvent(Guid cartGodosId)
        {
            CartGoodsId = cartGodosId;
        }
    }
}
