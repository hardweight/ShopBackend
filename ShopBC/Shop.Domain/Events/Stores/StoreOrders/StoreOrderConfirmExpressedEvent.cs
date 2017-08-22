using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Stores.StoreOrders
{
    [Serializable]
    public class StoreOrderConfirmExpressedEvent:DomainEvent<Guid>
    {
        public Guid StoreOwnerWalletId { get;private set; }
        public decimal StoreGetAmount { get;private  set; }

        public StoreOrderConfirmExpressedEvent() { }
        public StoreOrderConfirmExpressedEvent(Guid storeOwnerWalletId,decimal storeGetAmount)
        {
            StoreOwnerWalletId = storeOwnerWalletId;
            StoreGetAmount = storeGetAmount;
        }

    }
}
