using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Categorys
{
    public class CategoryCreatedEvent:DomainEvent<Guid>
    {
        public Guid ParentId { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string Thumb { get; private set; }


        public CategoryCreatedEvent() { }
        public CategoryCreatedEvent(Guid parentId,string name,string url,string thumb)
        {
            ParentId = parentId;
            Name = name;
            Url = url;
            Thumb = thumb;
        }
    }
}
