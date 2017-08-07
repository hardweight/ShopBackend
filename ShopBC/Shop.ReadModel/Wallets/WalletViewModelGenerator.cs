using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using Shop.Domain.Events.Wallets;
using Shop.Domain.Events.Wallets.BankCards;
using System.Threading.Tasks;

namespace Shop.ReadModel.Wallets
{
    /// <summary>
    /// 获取领域事件更新读库 基于Dapper
    /// </summary>
    [Component]
    public class WalletViewModelGenerator: BaseGenerator,
        IMessageHandler<WalletCreatedEvent>,

        IMessageHandler<NewCashTransferEvent>,
        IMessageHandler<NewBenevolenceTransferEvent>,

        IMessageHandler<WalletStatisticInfoChangedEvent>,
        IMessageHandler<WalletAccessCodeUpdatedEvent>,

        IMessageHandler<BankCardAddedEvent>,
        IMessageHandler<BankCardRemovedEvent>,
        IMessageHandler<BankCardUpdatedEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(WalletCreatedEvent evnt)
        {
            return TryInsertRecordAsync(connection =>
            {
                return connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    UserId = evnt.UserId,
                    AccessCode="",//交易密码默认为空，需用户后期自己设置
                    Cash = 0,
                    Benevolence =0,
                    YesterdayEarnings=0,
                    Earnings=0,
                    YesterdayIndex=0,
                    BenevolenceTotal=0,
                    TodayBenevolenceAdded=0,
                    CreatedOn = evnt.Timestamp,
                    UpdatedOn=evnt.Timestamp,
                    Version = evnt.Version,
                    EventSequence=evnt.Sequence
                }, ConfigSettings.WalletTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(NewCashTransferEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {

                return connection.UpdateAsync(new
                {
                    Version = evnt.Version,
                    Cash = evnt.FinallyValue,
                    UpdatedOn = evnt.Timestamp,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    UserId=evnt.UserId,
                    Version = evnt.Version - 1
                }, ConfigSettings.WalletTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(NewBenevolenceTransferEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Benevolence = evnt.FinallyValue,
                    UpdatedOn = evnt.Timestamp,
                    Version = evnt.Version,
                    EventSequence=evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    UserId = evnt.UserId,
                    Version = evnt.Version - 1
                }, ConfigSettings.WalletTable);
            });
        }

        public Task<AsyncTaskResult> HandleAsync(WalletStatisticInfoChangedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    YesterdayEarnings = evnt.StatisticInfo.YesterdayEarnings,
                    Earnings = evnt.StatisticInfo.Earnings,
                    YesterdayIndex = evnt.StatisticInfo.YesterdayIndex,
                    BenevolenceTotal = evnt.StatisticInfo.BenevolenceTotal,
                    TodayBenevolenceAdded=evnt.StatisticInfo.TodayBenevolenceAdded,
                    Version = evnt.Version,
                    EventSequence=evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.WalletTable);
            });
        }


        #region 银行卡
        /// <summary>
        /// 处理 添加
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(BankCardAddedEvent evnt)
        {
            return TryTransactionAsync(async (connection, transaction) =>
            {
                var effectedRows = await connection.UpdateAsync(new
                {
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.WalletTable, transaction);
                if (effectedRows == 1)
                {
                    await connection.InsertAsync(new
                    {
                        Id = evnt.BankCardId,
                        BankName = evnt.Info.BankName,
                        OwnerName=evnt.Info.OwnerName,
                        Number = evnt.Info.Number,
                        WalletId = evnt.AggregateRootId,
                        CreatedOn=evnt.Timestamp
                    }, ConfigSettings.BankCardTable, transaction);
                }
            });
        }
        /// <summary>
        /// 处理 更新
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(BankCardUpdatedEvent evnt)
        {
            return TryTransactionAsync(async (connection, transaction) =>
            {
                var effectedRows = await connection.UpdateAsync(new
                {
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.WalletTable, transaction);

                if (effectedRows == 1)
                {
                    await connection.UpdateAsync(new
                    {
                        BankName = evnt.Info.BankName,
                        OwnerName=evnt.Info.OwnerName,
                        Number = evnt.Info.Number,
                    }, new
                    {
                        WalletId = evnt.AggregateRootId,
                        Id = evnt.BankCardId
                    }, ConfigSettings.BankCardTable, transaction);
                }
            });
        }
        /// <summary>
        /// 处理 删除
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(BankCardRemovedEvent evnt)
        {
            return TryTransactionAsync(async (connection, transaction) =>
            {
                var effectedRows = await connection.UpdateAsync(new
                {
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.WalletTable, transaction);
                if (effectedRows == 1)
                {
                    await connection.DeleteAsync(new
                    {
                        WalletId = evnt.AggregateRootId,
                        Id = evnt.BankCardId
                    }, ConfigSettings.BankCardTable, transaction);
                }
            });
        }




        #endregion

        public Task<AsyncTaskResult> HandleAsync(WalletAccessCodeUpdatedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    AccessCode = evnt.AccessCode,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.WalletTable);
            });
        }
    }
}
