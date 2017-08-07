using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Categorys
{
    public class CategoryUpdatedEvent:DomainEvent<Guid>
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string Thumb { get; private set; }

        public CategoryUpdatedEvent() { }
        public CategoryUpdatedEvent(string name,string url,string thumb)
        {
            Name = name;
            Url = url;
        }
    }
}
