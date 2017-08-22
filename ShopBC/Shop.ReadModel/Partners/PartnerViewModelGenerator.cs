using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using Shop.Domain.Events.Partners;
using System.Threading.Tasks;

namespace Shop.ReadModel.Partners
{
    /// <summary>
    /// 更新读库 Dapper
    /// </summary>
    [Component]
    public class PartnerViewModelGenerator:BaseGenerator,
        IMessageHandler<PartnerCreatedEvent>,
        IMessageHandler<AcceptedNewSaleEvent>,
        IMessageHandler<ApplyBalancedEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(PartnerCreatedEvent evnt)
        {
            return TryInsertRecordAsync(connection =>
            {
                return connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    UserId = evnt.UserId,
                    WalletId = evnt.WalletId,
                    UnBalanceAmount = 0,
                    CreatedOn = evnt.Timestamp,
                    LastBanlanceTime = evnt.Timestamp,
                    Region = evnt.Region,//联盟地区
                    Province=evnt.Province,//联盟的层次关系
                    City=evnt.City,
                    County=evnt.County,
                    Level = (int)evnt.Level,
                    Version = evnt.Version,
                    EvntSequence=evnt.Sequence
                }, ConfigSettings.PartnerTable);
            });
        }

        public Task<AsyncTaskResult> HandleAsync(ApplyBalancedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    UnBalanceAmount = 0,
                    LastBanlanceTime = evnt.Timestamp,
                    Version = evnt.Version
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.WalletTable);
            });
        }

        public Task<AsyncTaskResult> HandleAsync(AcceptedNewSaleEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    UnBalanceAmount = evnt.Amount,
                    Version = evnt.Version
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.WalletTable);
            });
        }
    }
}
