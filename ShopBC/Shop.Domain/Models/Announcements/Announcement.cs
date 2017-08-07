﻿using ENode.Domain;
using Shop.Domain.Events.Announcements;
using System;
using Xia.Common.Extensions;

namespace Shop.Domain.Models.Announcements
{
    /// <summary>
    /// 系统公告
    /// </summary>
    public class Announcement:AggregateRoot<Guid>
    {
        public string _title;
        public string _body;

        public Announcement(Guid id,string title,string body):base(id)
        {
            title.CheckNotNullOrEmpty(nameof(title));
            body.CheckNotNullOrEmpty(nameof(body));

            ApplyEvent(new AnnouncementCreatedEvent(title, body));
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="title"></param>
        /// <param name="body"></param>
        public void Update(string title,string body)
        {
            ApplyEvent(new AnnouncementUpdatedEvent(title,body));
        }


        #region Handle


        private void Handle(AnnouncementCreatedEvent evnt)
        {
            _title = evnt.Title;
            _body = evnt.Body;
        }
        private void Handle(AnnouncementUpdatedEvent evnt)
        {
            _title = evnt.Title;
            _body = evnt.Body;
        }
        #endregion
    }
}