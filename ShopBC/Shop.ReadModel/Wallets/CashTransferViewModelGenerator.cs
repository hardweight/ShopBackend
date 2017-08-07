using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using Shop.Domain.Events.Wallets.CashTransfers;
using System.Threading.Tasks;

namespace Shop.ReadModel.Wallets
{
    /// <summary>
    /// 获取领域事件更新读库 基于Dapper
    /// </summary>
    [Component]
    public class CashTransferViewModelGenerator: BaseGenerator,
        IMessageHandler<CashTransferCreatedEvent>,
        IMessageHandler<CashTransferStatusChangedEvent>
       
    {
        public Task<AsyncTaskResult> HandleAsync(CashTransferCreatedEvent evnt)
        {
            return TryInsertRecordAsync(connection =>
            {
                return connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    WalletId=evnt.WalletId,
                    Number= evnt.Number,
                    Amount=evnt.Info.Amount,
                    Fee = evnt.Info.Fee,
                    Direction = (int)evnt.Info.Direction,
                    Remark = evnt.Info.Remark,
                    Type =(int)evnt.Type,
                    Status = (int)evnt.Status,
                    CreatedOn = evnt.Timestamp,
                    Version = evnt.Version,
                    EventSequence=evnt.Sequence
                }, ConfigSettings.CashTransferTable);
            });
        }

        public Task<AsyncTaskResult> HandleAsync(CashTransferStatusChangedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Status = (int)evnt.Status,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.CashTransferTable);
            });
        }
    }
}
