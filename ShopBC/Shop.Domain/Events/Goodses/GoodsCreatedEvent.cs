using Shop.Domain.Models.Goodses;
using System;

namespace Shop.Domain.Events.Goodses
{
    [Serializable]
    public class GoodsCreatedEvent:GoodsEvent
    {
        public Guid StoreId { get; private set; }

        public GoodsCreatedEvent() { }
        public GoodsCreatedEvent(Guid storeId,GoodsInfo info):base(info)
        {
            StoreId = storeId;
        }
    }
}
