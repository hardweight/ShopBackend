using ECommon.Components;
using Shop.ReadModel.Announcements.Dtos;
using System.Collections.Generic;
using Dapper;
using ECommon.Dapper;
using Shop.Common;
using System.Linq;
using System;

namespace Shop.ReadModel.Announcements
{
    /// <summary>
    /// 查询服务 实现
    /// </summary>
    [Component]
    public class AnnouncementQueryService : BaseQueryService, IAnnouncementQueryService
    {
        public Announcement Find(Guid id)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Announcement>(new { Id = id }, ConfigSettings.AnnouncementTable).SingleOrDefault();
            }
        }

        public IEnumerable<Announcement> ListPage()
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Announcement>(new {  }, ConfigSettings.AnnouncementTable);
            }
        }

        
    }
}