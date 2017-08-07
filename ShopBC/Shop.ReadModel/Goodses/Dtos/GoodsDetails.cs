using System;

namespace Shop.ReadModel.Goodses.Dtos
{
    public class GoodsDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        
        public DateTime CreatedOn { get; set; }
    }
}
