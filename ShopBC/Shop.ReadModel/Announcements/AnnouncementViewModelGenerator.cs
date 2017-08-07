using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using Shop.Domain.Events.Announcements;
using System.Threading.Tasks;

namespace Shop.ReadModel.Announcements
{
    /// <summary>
    /// 更新读库 Dapper
    /// </summary>
    [Component]
    public class AnnouncementViewModelGenerator : BaseGenerator,
        IMessageHandler<AnnouncementCreatedEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(AnnouncementCreatedEvent evnt)
        {
            return TryInsertRecordAsync(connection =>
            {
                return connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    Title = evnt.Title,
                    Body = evnt.Body,
                    CreatedOn = evnt.Timestamp,
                    Version = evnt.Version,
                    EventSequence=evnt.Sequence
                }, ConfigSettings.AnnouncementTable);
            });
        }
        
    }
}
