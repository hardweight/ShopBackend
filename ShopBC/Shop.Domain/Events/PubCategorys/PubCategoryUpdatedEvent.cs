﻿using ENode.Eventing;
using System;

namespace Shop.Domain.Events.PubCategorys
{
    public class PubCategoryUpdatedEvent:DomainEvent<Guid>
    {
        public string Name { get; private set; }
        public string Thumb { get; private set; }
        public int Sort { get; private set; }

        public PubCategoryUpdatedEvent() { }
        public PubCategoryUpdatedEvent(string name,string thumb,int sort)
        {
            Name = name;
            Thumb = thumb;
            Sort = sort;
        }
    }
}