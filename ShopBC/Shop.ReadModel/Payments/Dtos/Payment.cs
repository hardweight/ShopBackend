using System;

namespace Shop.ReadModel.Payments.Dtos
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        //public Guid GoodsId { get; set; }
        public int State { get; set; }
        public int TotalAmount { get; set; }
        public string Description { get; set; }
    }
}
