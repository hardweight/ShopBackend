using System;

namespace Shop.ReadModel.Goodses.Dtos
{
    public class GoodsParam
    {
        public Guid Id { get; set; }
        public Guid GoodsId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
