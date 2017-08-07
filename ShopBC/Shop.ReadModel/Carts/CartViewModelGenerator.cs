using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using Shop.Domain.Events.Carts;
using System.Threading.Tasks;

namespace Shop.ReadModel.Carts
{
    /// <summary>
    /// 更新读库 Dapper
    /// </summary>
    [Component]
    public class CartViewModelGenerator : BaseGenerator,
        IMessageHandler<CartCreatedEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(CartCreatedEvent evnt)
        {
            return TryInsertRecordAsync(connection =>
            {
                return connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    UserId = evnt.UserId,
                    Version = evnt.Version,
                    EventSequence=evnt.Sequence
                }, ConfigSettings.CartTable);
            });
        }
        
    }
}
