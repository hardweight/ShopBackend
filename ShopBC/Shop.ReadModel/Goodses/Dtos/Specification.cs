using System;

namespace Shop.ReadModel.Goodses.Dtos
{
    public class Specification
    {
        public Guid Id { get; set; }
        public Guid GoodsId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
