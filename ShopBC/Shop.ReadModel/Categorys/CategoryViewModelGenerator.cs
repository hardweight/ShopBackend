using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using Shop.Domain.Events.Categorys;
using System.Threading.Tasks;

namespace Shop.ReadModel.Categorys
{
    /// <summary>
    /// 更新读库 Dapper
    /// </summary>
    [Component]
    public class CategoryViewModelGenerator:BaseGenerator,
        IMessageHandler<CategoryCreatedEvent>
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
                    Url = evnt.Url,
                    CreatedOn = evnt.Timestamp,
                    Version = evnt.Version,
                    EventSequence=evnt.Sequence
                }, ConfigSettings.CategoryTable);
            });
        }
        
    }
}
