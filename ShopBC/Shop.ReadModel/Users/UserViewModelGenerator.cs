using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using Shop.Domain.Events.Users;
using System.Threading.Tasks;
using System;
using Shop.Domain.Models.Users;
using Shop.Domain.Events.Users.ExpressAddresses;
using Shop.Domain.Events.Users.UserGifts;
using Shop.Common.Enums;

namespace Shop.ReadModel.Users
{
    /// <summary>
    /// 获取领域事件更新读库 基于Dapper
    /// </summary>
    [Component]
    public class UserViewModelGenerator: BaseGenerator,
        IMessageHandler<UserCreatedEvent>,
        IMessageHandler<UserEditedEvent>,
        IMessageHandler<UserNickNameUpdatedEvent>,
        IMessageHandler<UserPasswordUpdatedEvent>,
        IMessageHandler<UserPortraitUpdatedEvent>,
        IMessageHandler<UserRegionUpdatedEvent>,
        IMessageHandler<UserGenderUpdatedEvent>,

        IMessageHandler<ExpressAddressAddedEvent>,
        IMessageHandler<ExpressAddressRemovedEvent>,
        IMessageHandler<ExpressAddressUpdatedEvent>,

        IMessageHandler<UserGiftAddedEvent>,
        IMessageHandler<UserGiftPayedEvent>,
        IMessageHandler<UserGiftRemarkChangedEvent>,

        IMessageHandler<UserLockedEvent>,
        IMessageHandler<UserUnLockedEvent>,
        IMessageHandler<UserFreezeEvent>,
        IMessageHandler<UserUnFreezeEvent>,

        IMessageHandler<RegionPartnerApplyedEvent>,

        IMessageHandler<UserRoleToAmbassadorEvent>,
        IMessageHandler<UserRoleToPartnerEvent>,
        IMessageHandler<UserRoleToPasserEvent>
    {

        /// <summary>
        /// 处理创建用户事件
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserCreatedEvent evnt)
        {
            return TryInsertRecordAsync(connection =>
            {
                var info = evnt.Info;
                return connection.InsertAsync(new
                {
                    Id = evnt.AggregateRootId,
                    WalletId=evnt.WalletId,
                    CartId=evnt.CartId,
                    Mobile = info.Mobile,
                    NickName = info.NickName,
                    Portrait = info.Portrait,
                    Password = info.Password,
                    Gender=info.Gender,
                    Region = info.Region,
                    Role=(int)UserRole.Consumer,
                    IsLocked = 0,
                    IsFreeze = 0,
                    CreatedOn = evnt.Timestamp,
                    AmbassadorExpireTime=evnt.Timestamp,
                    WeixinId=info.WeixinId,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, ConfigSettings.UserTable);
            });
        }


        #region 基本信息

        /// <summary>
        /// 处理 更新昵称事件
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserNickNameUpdatedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    NickName = evnt.NickName,                    
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        /// <summary>
        /// 处理 更新头像事件
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserPortraitUpdatedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Portrait = evnt.Portrait,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        /// <summary>
        /// 处理 更新性别事件
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserGenderUpdatedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Gender = evnt.Gender,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        /// <summary>
        /// 处理 更新密码事件
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserPasswordUpdatedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Password = evnt.Password,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        /// <summary>
        /// 处理 更新地区事件
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(UserRegionUpdatedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Region = evnt.Region,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(UserLockedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    IsLocked = (int)UserLock.Locked,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(UserUnLockedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    IsLocked = (int)UserLock.UnLocked,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(UserFreezeEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    IsFreeze = (int)UserFreeze.Freeze,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(UserUnFreezeEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    IsFreeze = (int)UserFreeze.UnFreeze,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        #endregion

        #region 快递地址
        /// <summary>
        /// 处理 添加快递地址
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(ExpressAddressAddedEvent evnt)
        {
            return TryTransactionAsync(async (connection, transaction) =>
            {
                //尽管是更新ExpressAddresssTable但是也要更新聚合跟，因为地址属于聚合跟的状态
                var effectedRows = await connection.UpdateAsync(new
                {
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable, transaction);
                if (effectedRows == 1)
                {
                    await connection.InsertAsync(new
                    {
                        Id = evnt.ExpressAddressId,
                        Name = evnt.Info.Name,
                        Region = evnt.Info.Region,
                        Address = evnt.Info.Address,
                        Zip = evnt.Info.Zip,
                        Mobile = evnt.Info.Mobile,
                        UserId = evnt.AggregateRootId,
                        CreatedOn = evnt.Timestamp
                    }, ConfigSettings.ExpressAddressTable, transaction);
                }
            });
        }
        /// <summary>
        /// 处理 更新快递地址
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(ExpressAddressUpdatedEvent evnt)
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
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable, transaction);

                if (effectedRows == 1)
                {
                    await connection.UpdateAsync(new
                    {
                        Name = evnt.Info.Name,
                        Region = evnt.Info.Region,
                        Address = evnt.Info.Address,
                        Zip=evnt.Info.Zip
                    }, new
                    {
                        UserId = evnt.AggregateRootId,
                        Id = evnt.ExpressAddressId
                    }, ConfigSettings.ExpressAddressTable, transaction);
                }
            });
        }
        /// <summary>
        /// 处理 删除更新地址
        /// </summary>
        /// <param name="evnt"></param>
        /// <returns></returns>
        public Task<AsyncTaskResult> HandleAsync(ExpressAddressRemovedEvent evnt)
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
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable, transaction);
                if (effectedRows == 1)
                {
                    await connection.DeleteAsync(new
                    {
                        UserId = evnt.AggregateRootId,
                        Id = evnt.ExpressAddressId
                    }, ConfigSettings.ExpressAddressTable, transaction);
                }
            });
        }


        #endregion

        #region 用户角色
        public Task<AsyncTaskResult> HandleAsync(RegionPartnerApplyedEvent evnt)
        {
            return TryTransactionAsync(async (connection, transaction) =>
            {
                //尽管是更新ExpressAddresssTable但是也要更新聚合跟，因为地址属于聚合跟的状态
                var effectedRows = await connection.UpdateAsync(new
                {
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable, transaction);
                if (effectedRows == 1)
                {
                    await connection.InsertAsync(new
                    {
                        UserId = evnt.AggregateRootId,
                        Region = evnt.Region,
                        Level = (int)evnt.Level,
                        CreatedOn = evnt.Timestamp,
                    }, ConfigSettings.PartnerApplyTable, transaction);
                }
            });
        }
        public Task<AsyncTaskResult> HandleAsync(UserRoleToAmbassadorEvent evnt)
        {
            if (evnt.OnlyUpdateTime)
            {//大使只更新时间
                return TryUpdateRecordAsync(connection =>
                {
                    return connection.UpdateAsync(new
                    {
                        AmbassadorExpireTime = evnt.ExpireTime,
                        Version = evnt.Version,
                        EventSequence = evnt.Sequence
                    }, new
                    {
                        Id = evnt.AggregateRootId,
                        //Version = evnt.Version - 1
                    }, ConfigSettings.UserTable);
                });
            }
            else
            {
                return TryUpdateRecordAsync(connection =>
                {
                    return connection.UpdateAsync(new
                    {
                        Role = (int)UserRole.Ambassador,
                        AmbassadorExpireTime = evnt.ExpireTime,
                        Version = evnt.Version,
                        EventSequence = evnt.Sequence
                    }, new
                    {
                        Id = evnt.AggregateRootId,
                        //Version = evnt.Version - 1
                    }, ConfigSettings.UserTable);
                });
            }
        }
        public Task<AsyncTaskResult> HandleAsync(UserRoleToPasserEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Role = (int)UserRole.Passer,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(UserRoleToPartnerEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Role = (int)UserRole.RegionPartner,
                    PartnerRegion=evnt.Region,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }

        public Task<AsyncTaskResult> HandleAsync(UserGiftAddedEvent evnt)
        {
            return TryTransactionAsync(async (connection, transaction) =>
            {
                //尽管是更新ExpressAddresssTable但是也要更新聚合跟，因为地址属于聚合跟的状态
                var effectedRows = await connection.UpdateAsync(new
                {
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable, transaction);
                if (effectedRows == 1)
                {
                    await connection.InsertAsync(new
                    {
                        Id = evnt.UserGiftId,
                        UserId = evnt.AggregateRootId,
                        GiftName =evnt.GiftInfo.Name,
                        GiftSize=evnt.GiftInfo.Size,
                        Name = evnt.ExpressAddressInfo.Name,
                        Region = evnt.ExpressAddressInfo.Region,
                        Address = evnt.ExpressAddressInfo.Address,
                        Zip = evnt.ExpressAddressInfo.Zip,
                        Mobile = evnt.ExpressAddressInfo.Mobile,
                        Remark=evnt.Remark,
                        CreatedOn=evnt.Timestamp
                    }, ConfigSettings.UserGiftTable, transaction);
                }
            });
        }

        public Task<AsyncTaskResult> HandleAsync(UserGiftPayedEvent evnt)
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
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable, transaction);
                if (effectedRows == 1)
                {
                    await connection.UpdateAsync(new
                    {
                        Remark="未发货"
                    }, new
                    {
                        UserId = evnt.AggregateRootId,
                        Id = evnt.UserGiftId
                    }, ConfigSettings.UserGiftTable, transaction);
                }
            });
        }

        public Task<AsyncTaskResult> HandleAsync(UserGiftRemarkChangedEvent evnt)
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
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable, transaction);
                if (effectedRows == 1)
                {
                    await connection.UpdateAsync(new
                    {
                        Remark = evnt.Remark
                    }, new
                    {
                        UserId = evnt.AggregateRootId,
                        Id = evnt.UserGiftId
                    }, ConfigSettings.UserGiftTable, transaction);
                }
            });
        }

        public Task<AsyncTaskResult> HandleAsync(UserEditedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    NickName = evnt.NickName,
                    Gender=evnt.Gender,
                    Version = evnt.Version,
                    EventSequence = evnt.Sequence
                }, new
                {
                    Id = evnt.AggregateRootId,
                    //Version = evnt.Version - 1
                }, ConfigSettings.UserTable);
            });
        }
        #endregion
    }
}
