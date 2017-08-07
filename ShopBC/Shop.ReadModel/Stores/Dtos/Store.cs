using System;

namespace Shop.ReadModel.Stores.Dtos
{
    public class Store
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string AccessCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }

    }
}
