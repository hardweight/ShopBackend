

namespace Shop.ReadModel.Goodses.Dtos
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class Goods
    {
        public Goods(Guid id, string code, string name, string description, decimal price, DateTimeOffset createdOn, IEnumerable<Specification> specifications)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            CreatedOn = createdOn;
        }
        public Goods() { }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedOn { get; set; }

        public bool IsPublished { get; set; }
    }
}
