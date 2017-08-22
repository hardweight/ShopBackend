using ENode.Eventing;
using Shop.Common.Enums;
using System;

namespace Shop.Domain.Events.Categorys
{
    public class CategoryUpdatedEvent:DomainEvent<Guid>
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string Thumb { get; private set; }
        public CategoryType Type { get; private set; }
        public int Sort { get; private set; }

        public CategoryUpdatedEvent() { }
        public CategoryUpdatedEvent(string name,string url,string thumb,CategoryType type,int sort)
        {
            Name = name;
            Url = url;
            Thumb = thumb;
            Type = type;
            Sort = sort;
        }
    }
}
