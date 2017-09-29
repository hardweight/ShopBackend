﻿using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Announcements
{
    public class AnnouncementDeletedEvent:DomainEvent<Guid>
    {
        public AnnouncementDeletedEvent() { }
    }
}
