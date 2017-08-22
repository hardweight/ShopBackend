using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using Shop.Domain.Events.Categorys;
using System.Threading.Tasks;
using System;

namespace Shop.ReadModel.Categorys
{
    /// <summary>
    /// 更新读库 Dapper
    /// </summary>
    [Component]
    public class CategoryViewModelGenerator:BaseGenerator,
        IMessageHandler<CategoryCreatedEvent>,
        IMessageHandler<CategoryUpdatedEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(CategoryCreatedEvent evnt)
        {
            return TryInsertRecordAsync(connection =>
            {
                return connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    ParentId = evnt.ParentId,
                    Name = evnt.Name,
                    Thumb=evnt.Thumb,
                    Type=(int)evnt.Type,
                    Url = evnt.Url,
                    Sort=evnt.Sort,
                    CreatedOn = evnt.Timestamp,
                    Version = evnt.Version,
                    EventSequence=evnt.Sequence
                }, ConfigSettings.CategoryTable);
            });
        }

        public Task<AsyncTaskResult> HandleAsync(CategoryUpdatedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Name = evnt.Name,
                    Url = evnt.Url,
                    Thumb=evnt.Thumb,
                    Type = (int)evnt.Type,
                    Sort =evnt.Sort,
                    Version = evnt.Version
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.CategoryTable);
            });
        }
    }
}
