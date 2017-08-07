using System;

namespace Shop.Messages.Goodses
{
    /// <summary>
    /// 预定项目信息 消息用
    /// </summary>
    [Serializable]
    public class SpecificationReservationItem
    {
        public Guid SpecificationId { get;private set; }
        public int Quantity { get;private set; }

        public SpecificationReservationItem() { }
        public SpecificationReservationItem(Guid specificationId,int quantity)
        {
            SpecificationId = specificationId;
            Quantity = quantity;
        }
    }
}
