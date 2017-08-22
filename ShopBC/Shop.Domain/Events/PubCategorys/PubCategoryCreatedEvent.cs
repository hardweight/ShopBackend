using ENode.Eventing;
using System;

namespace Shop.Domain.Events.PubCategorys
{
    public class PubCategoryCreatedEvent:DomainEvent<Guid>
    {
        public Guid ParentId { get; private set; }
        public string Name { get; private set; }
        public string Thumb { get; private set; }
        public int Sort { get; private set; }


        public PubCategoryCreatedEvent() { }
        public PubCategoryCreatedEvent(Guid parentId,string name,string thumb,int sort)
        {
            ParentId = parentId;
            Name = name;
            Thumb = thumb;
            Sort = sort;
        }
    }
}
