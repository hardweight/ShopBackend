using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using Shop.Domain.Events.Stores;
using Shop.Domain.Models.Stores;
using System.Threading.Tasks;

namespace Shop.ReadModel.Stores
{
    /// <summary>
    /// 更新读库 Dapper
    /// </summary>
    [Component]
    public class StoreViewModelGenerator:BaseGenerator,
        IMessageHandler<StoreCreatedEvent>,
        IMessageHandler<StoreUpdatedEvent>,
        IMessageHandler<StoreLockedEvent>,
        IMessageHandler<StoreUnLockedEvent>,
        IMessageHandler<StoreStatisticInfoChangedEvent>,

        IMessageHandler<SubjectInfoUpdatedEvent>,
        IMessageHandler<StoreAccessCodeUpdatedEvent>,
        IMessageHandler<StoreStatusUpdatedEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(StoreCreatedEvent evnt)
        {
            return TryInsertRecordAsync(connection =>
            {
                var info = evnt.Info;
                return connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    UserId = evnt.UserId,
                    AccessCode=info.AccessCode,
                    Name = info.Name,
                    Description = info.Description,
                    Region = info.Region,
                    Address = info.Address,
                    SubjectName=evnt.SubjectInfo.SubjectName,
                    SubjectNumber=evnt.SubjectInfo.SubjectNumber,
                    SubjectPic=evnt.SubjectInfo.SubjectPic,
                    CreatedOn=evnt.Timestamp,
                    Status=(int)StoreStatus.Apply,
                    IsLocked = 0,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, ConfigSettings.StoreTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(StoreUpdatedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                var info = evnt.Info;
                return connection.UpdateAsync(new
                {
                    Name = info.Name,
                    Description = info.Description,
                    Address = info.Address,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.StoreTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(StoreLockedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    IsLocked = 1,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.StoreTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(StoreUnLockedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    IsLocked = 0,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.StoreTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(StoreStatisticInfoChangedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    TodaySale = evnt.StatisticInfo.TodaySale,
                    TotalSale = evnt.StatisticInfo.TotalSale,
                    OnSaleGoodsCount = evnt.StatisticInfo.OnSaleGoodsCount,
                    Version = evnt.Version
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.StoreTable);
            });
        }

        public Task<AsyncTaskResult> HandleAsync(SubjectInfoUpdatedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                var info = evnt.SubjectInfo;
                return connection.UpdateAsync(new
                {
                    SubjectName = info.SubjectName,
                    SubjectNumber = info.SubjectNumber,
                    SubjectPic = info.SubjectPic,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.StoreTable);
            });
        }

        public Task<AsyncTaskResult> HandleAsync(StoreAccessCodeUpdatedEvent evnt)
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
                }, ConfigSettings.StoreTable);
            });
        }

        public Task<AsyncTaskResult> HandleAsync(StoreStatusUpdatedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Status =(int)evnt.Status,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.StoreTable);
            });
        }
    }
}
