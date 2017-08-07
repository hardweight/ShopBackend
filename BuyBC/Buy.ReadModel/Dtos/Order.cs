using System;
using System.Collections.Generic;

namespace Buy.ReadModel.Dtos
{
    public class Order
    {
        private IList<OrderLine> _lines = new List<OrderLine>();

        public Guid OrderId { get; set; }
        public Guid GoodsId { get; set; }
        public int Status { get; set; }
        public string RegistrantEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? ReservationExpirationDate { get; set; }

        public bool IsFreeOfCharge()
        {
            return TotalAmount == 0;
        }

        public void SetLines(IList<OrderLine> lines)
        {
            _lines = lines;
        }
        public IList<OrderLine> GetLines()
        {
            return _lines;
        }
    }
    
}
