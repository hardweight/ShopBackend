using System;
using ENode.Domain;
using System.Collections.Generic;
using Shop.Domain.Events.Stores;
using Shop.Common;
using Xia.Common.Extensions;
using System.Linq;

namespace Shop.Domain.Models.Stores
{
    /// <summary>
    /// 店铺聚合跟
    /// </summary>
    public class Store : AggregateRoot<Guid>
    {
        private StoreInfo _info;//基本信息
        private SubjectInfo _subjectInfo;//主体信息
        private Guid _userId;//所有人 必须是交年费的传递大使 身份失效店铺也失效？
        private Guid _sectionId;//所属行业
        private bool _isLocked;//店铺可单独锁定
        private ISet<Guid> _orderIds;
        private StoreStatisticInfo _storeStatisticInfo;//店铺统计信息
        private StoreStatus _status;//店铺状态

        /// <summary>
        /// 创建店铺
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="sectionId"></param>
        /// <param name="info"></param>
        /// <param name="subjectInfo"></param>
        public Store(Guid id, Guid userId,StoreInfo info,SubjectInfo subjectInfo) : base(id)
        {
            userId.CheckNotEmpty(nameof(userId));
            ApplyEvent(new StoreCreatedEvent(userId,info,subjectInfo));
        }

        /// <summary>
        /// 设置管理密码
        /// </summary>
        /// <param name="accessCode"></param>
        public void SetAccessCode(string accessCode)
        {
            ApplyEvent(new StoreAccessCodeUpdatedEvent(accessCode));
        }

        /// <summary>
        /// 更新店铺状态
        /// </summary>
        /// <param name="status"></param>
        public void UpdateStatus(StoreStatus status)
        {
            ApplyEvent(new StoreStatusUpdatedEvent(status));
        }

        /// <summary>
        /// 更新主体信息
        /// </summary>
        /// <param name="subjectInfo"></param>
        public void UpdateSubjectInfo(SubjectInfo subjectInfo)
        {
            subjectInfo.CheckNotNull(nameof(subjectInfo));
            ApplyEvent(new SubjectInfoUpdatedEvent(subjectInfo));
        }

        /// <summary>
        /// 更新店铺-更新基本信息
        /// </summary>
        /// <param name="info"></param>
        public void Update(StoreEditableInfo info)
        {
            ApplyEvent(new StoreUpdatedEvent(info));
        }

        

        /// <summary>
        /// 接受新订单用户下单时 更新统计信息
        /// </summary>
        /// <param name="sale"></param>
        public void AcceptNewOrder(StoreOrder storeOrder)
        {
            if (!_orderIds.Add(storeOrder.Id)) return;
            if (_storeStatisticInfo == null)
            {
                ApplyEvent(new StoreStatisticInfoChangedEvent(new StoreStatisticInfo(
                    storeOrder.GetTotal(),
                    storeOrder.GetTotal(),
                    0)));
            }
            else
            {
                ApplyEvent(new StoreStatisticInfoChangedEvent(new StoreStatisticInfo(
                    _storeStatisticInfo.TodaySale+ storeOrder.GetTotal(),
                    _storeStatisticInfo.TotalSale+ storeOrder.GetTotal(),
                    0)));
            }
        }

        /// <summary>
        /// 接受商品的上下架信息，更新商家统计
        /// </summary>
        /// <param name="isOnSale"></param>
        public void AcceptGoodsOnOffSale(bool isOnSale)
        {
            if (_storeStatisticInfo == null)
            {
                ApplyEvent(new StoreStatisticInfoChangedEvent(new StoreStatisticInfo(
                    0,
                    0,
                    1)));
            }
            else if (isOnSale)
            {
                ApplyEvent(new StoreStatisticInfoChangedEvent(new StoreStatisticInfo(
                    _storeStatisticInfo.TodaySale,
                    _storeStatisticInfo.TotalSale,
                    _storeStatisticInfo.OnSaleGoodsCount + 1
                    )));
            }
            else
            {
                ApplyEvent(new StoreStatisticInfoChangedEvent(new StoreStatisticInfo(
                    _storeStatisticInfo.TodaySale,
                    _storeStatisticInfo.TotalSale,
                    _storeStatisticInfo.OnSaleGoodsCount - 1
                    )));
            }
        }


        /// <summary>
        /// 锁定
        /// </summary>
        public void Lock()
        {
            if (_isLocked)
            {
                throw new Exception("Store already Locked.");
            }
            ApplyEvent(new StoreLockedEvent());
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public void UnLock()
        {
            if (!_isLocked)
            {
                throw new Exception("Store already unlocked.");
            }
            ApplyEvent(new StoreUnLockedEvent());
        }

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        /// <returns></returns>
        public StoreInfo GetInfo()
        {
            return _info;
        }

        public SubjectInfo GetSubjectInfo()
        {
            return _subjectInfo;
        }

        #region Event Handler
        private void Handle(StoreCreatedEvent evnt)
        {
            _info = evnt.Info;
            _subjectInfo = evnt.SubjectInfo;
            _userId = evnt.UserId;
            _sectionId = Guid.Empty;
            _orderIds = new HashSet<Guid>();
            _isLocked = false;
            _status = StoreStatus.Apply;
        }
        private void Handle(StoreStatusUpdatedEvent evnt)
        {
            _status = evnt.Status;
        }
        private void Handle(StoreAccessCodeUpdatedEvent evnt)
        {
            _info.AccessCode = evnt.AccessCode;
        }
        private void Handle(StoreUpdatedEvent evnt)
        {
            var editableInfo = evnt.Info;
            _info = new StoreInfo(
                _info.AccessCode,
                editableInfo.Name, 
                editableInfo.Description,
                _info.Region,
                editableInfo.Address);
        }
        private void Handle(SubjectInfoUpdatedEvent evnt)
        {
            _subjectInfo = evnt.SubjectInfo;
        }
        private void Handle(StoreLockedEvent evnt)
        {
            _isLocked = true;
        }
        private void Handle(StoreUnLockedEvent evnt)
        {
            _isLocked = false;
        }
        private void Handle(StoreStatisticInfoChangedEvent evnt)
        {
            _storeStatisticInfo = evnt.StatisticInfo;
        }
        #endregion
    }
}
