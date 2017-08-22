using ENode.Domain;
using Shop.Common;
using Shop.Common.Enums;
using Shop.Domain.Events.Users;
using Shop.Domain.Events.Users.ExpressAddresses;
using Shop.Domain.Events.Users.UserGifts;
using Shop.Domain.Models.Users.ExpressAddresses;
using Shop.Domain.Models.Users.UserGifts;
using System;
using System.Collections.Generic;
using System.Linq;
using Xia.Common.Extensions;

namespace Shop.Domain.Models.Users
{
    /// <summary>
    /// 用户聚合跟
    /// </summary>
    public class User:AggregateRoot<Guid>
    {
        private Guid _parentId;//推荐人
        private UserInfo _info;//用户信息
        private IList<ExpressAddress> _expressAddresses;
        private IList<UserGift> _userGifts;
        private Guid _walletId;//用户钱包ID
        private Guid _cartId;//购物车ID
        private Guid _storeId;//我的店铺ID  还没获取
        private bool _isLocked;//是否锁定账号 只用于限制登陆
        private bool _isFreeze;//是否冻结账号 怀疑账号被盗可以冻结账号
        private UserRole _role;//用户角色
        private ISet<Guid> _myRecommends;//保存我推荐的用户
        private decimal _mySpending;//我的消费额
        private DateTime _ambassadorExpireTime;//大使身份过期时间


        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        public User(Guid id, User parent, UserInfo info) : base(id)
        {
            if (parent != null && id == parent.Id)
            {
                throw new ArgumentException(string.Format("用户的推荐人不能是自己，Id:{0}", id));
            }
            ApplyEvent(new UserCreatedEvent(info, parent == null ? Guid.Empty : parent.Id,Guid.NewGuid(),Guid.NewGuid()));
        }

        #region 基本信息修改
        public void Edit(string nickName,string gender)
        {
            nickName.CheckNotNullOrEmpty(nameof(nickName));
            if (nickName.Length > 20)
            {
                throw new Exception("昵称不得超过20字符");
            }
            if (!"男,女,保密".IsIncludeItem(gender))
            {
                throw new Exception("只接受参数值：男/女/保密");
            }
            ApplyEvent(new UserEditedEvent(nickName,gender));
        }
        /// <summary>
        /// 更新昵称
        /// </summary>
        /// <param name="nickName"></param>
        public void UpdateNickName(string nickName )
        {
            //Assert.IsNotNullOrWhiteSpace("昵称", nickName);
            nickName.CheckNotNullOrEmpty(nameof(nickName));
            if(nickName.Length>20)
            {
                throw new Exception("昵称不得超过20字符");
            }
            ApplyEvent(new UserNickNameUpdatedEvent(nickName));
        }

        


        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="password">HASHE</param>
        public void UpdatePassword(string password)
        {
            password.CheckNotNullOrEmpty(nameof(password));
            ApplyEvent(new UserPasswordUpdatedEvent(password));
        }

        /// <summary>
        /// 更新性别
        /// </summary>
        /// <param name="gender"></param>
        public void UpdateGender(string gender)
        {
            gender.CheckNotNullOrEmpty(nameof(gender));
            if (!"男,女,保密".IsIncludeItem(gender))
            {
                throw new Exception("只接受参数值：男/女/保密");
            }
            ApplyEvent(new UserGenderUpdatedEvent(gender));
        }
        /// <summary>
        /// 更新地区
        /// </summary>
        /// <param name="region"></param>
        public void UpdateRegion(string region)
        {
            region.CheckNotNullOrEmpty(nameof(region));  
            ApplyEvent(new UserRegionUpdatedEvent(region));
        }
        /// <summary>
        /// 更新头像
        /// </summary>
        /// <param name="portrait"></param>
        public void UpdatePortrait(string portrait)
        {
            portrait.CheckNotNullOrEmpty(nameof(portrait));        
            ApplyEvent(new UserPortraitUpdatedEvent(portrait));
        }

        /// <summary>
        /// 锁定用户 限制登陆
        /// </summary>
        public void Lock()
        {
            if (_isLocked)
            {
                throw new Exception("用户早已锁定.");
            }            
            ApplyEvent(new UserLockedEvent());
        }
        /// <summary>
        /// 解锁用户
        /// </summary>
        public void UnLock()
        {
            if (!_isLocked)
            {
                throw new Exception("用户早已解算");
            }
            ApplyEvent(new UserUnLockedEvent());
        }

        /// <summary>
        /// 冻结用户 限制交易
        /// </summary>
        public void Freeze()
        {
            if (_isFreeze)
            {
                throw new Exception("用户早已冻结.");
            }
            ApplyEvent(new UserFreezeEvent());
        }
        /// <summary>
        /// 解冻用户
        /// </summary>
        public void UnFreeze()
        {
            if (!_isFreeze)
            {
                throw new Exception("用户早已解冻");
            }
            ApplyEvent(new UserUnFreezeEvent());
        }

        #endregion

        #region 获取信息
        public Guid GetWalletId()
        {
            return _walletId;
        }
        #endregion

        #region 开通大使
        /// <summary>
        /// 缴费成为大使
        /// </summary>
        /// <param name="chargeAmount"></param>
        public void PayToAmbassador()
        {
            bool onlyupdatetime = false;
            if (_role == UserRole.Ambassador || _role == UserRole.RegionPartner || _role == UserRole.SectionPartner)
            {
                onlyupdatetime = true;
            }
            var expireTime = _ambassadorExpireTime.AddYears(1);
            ApplyEvent(new UserRoleToAmbassadorEvent(onlyupdatetime, expireTime));
        }

        /// <summary>
        /// 添加用户礼物 未支付
        /// </summary>
        /// <param name="giftInfo"></param>
        /// <param name="expressAddressInfo"></param>
        public void AddUserGift(Guid userGiftId,GiftInfo giftInfo,ExpressAddressInfo expressAddressInfo)
        {
            giftInfo.CheckNotNull(nameof(giftInfo));
            expressAddressInfo.CheckNotNull(nameof(expressAddressInfo));

            ApplyEvent(new UserGiftAddedEvent(userGiftId, giftInfo, expressAddressInfo));
        }

        /// <summary>
        /// 用户赠品支付成功
        /// </summary>
        public void SetUserGiftPayed(Guid userGiftId)
        {
            ApplyEvent(new UserGiftPayedEvent(userGiftId));
        }

        /// <summary>
        /// 更新用户物料 备注
        /// </summary>
        /// <param name="userGiftId"></param>
        /// <param name="remark"></param>
        public void SetUserGiftRemark(Guid userGiftId,string remark)
        {
            remark.CheckNotNullOrEmpty(nameof(remark));

            ApplyEvent(new UserGiftRemarkChangedEvent(userGiftId, remark));
        }
        #endregion

        #region 推荐用户
        /// <summary>
        /// 接受新的推荐者
        /// </summary>
        /// <param name="userId"></param>
        public void AcceptNewRecommend(Guid userId)
        {
            userId.CheckNotEmpty(nameof(userId));

            if (!_myRecommends.Add(userId)) return;

            //如果是善心使者 并且推荐人数等于10
            if(_role==UserRole.Consumer && _myRecommends.Count==ConfigSettings.ToPasserRecommendCount)
            {//更新用户级别为传递使者
                ApplyEvent(new UserRoleToPasserEvent());
            }
        }
        #endregion

        #region 用户消费结算逻辑
        /// <summary>
        /// 接受自己新的消费额 我的订单完成时
        /// </summary>
        /// <param name="amount">订单额</param>
        /// <param name="surrenderPersent">商品的让利比例</param>
        public void AcceptMyNewSpending(decimal amount,decimal surrender)
        {
            if (amount <= 0) return;
            ApplyEvent(new UserNewSpendingEvent(amount));

            //用户角色升级
            if(_mySpending>=ConfigSettings.ToPasserSpendingAmount && _role==UserRole.Consumer)
            {
                //更新用户级别为传递使者
                ApplyEvent(new UserRoleToPasserEvent());
            }

            //用户消费转换为善心
            if (ConfigSettings.BenevolenceValue <= 0)
            {
                throw new Exception("善心价值配置异常");
            }
            //消费获得的善心量 消费额* 返利倍数
            var benevolenceAmount = (amount * surrender) / ConfigSettings.BenevolenceValue;
            ApplyEvent(new UserSpendingTransformToBenevolenceEvent(_walletId, benevolenceAmount));
            

            //计算我的推荐者的 间接激励
            if(_parentId!=Guid.Empty)
            {
                //如果我有推荐者，将我消费的信息广播给我的推荐者推荐者自己计算自己的一度或二度激励
                ApplyEvent(new MyParentCanGetBenevolenceEvent(_parentId, benevolenceAmount, 1));
            }
        }

        /// <summary>
        /// 接受被推荐人的善心分成
        /// </summary>
        /// <param name="amount">被推荐人获得的善心量</param>
        /// <param name="level">层级</param>
        public void AcceptChildBenevolence(decimal amount,int level)
        {
            if (amount <= 0) return;
            if(level>0 && level<=2)//目前只接受一度二度奖励
            {
                if (level==1)//一度
                {
                    var myamount = Math.Round(amount * 0.05M,4);//我能获取的善心
                    if (_role != UserRole.Consumer)
                    {
                        //善心使者没有资格获取推荐奖励
                        ApplyEvent(new UserGetChildBenevolenceEvent(_walletId, myamount, level));
                    }
                    if(_parentId!=Guid.Empty)
                    {
                        //继续递归
                        ApplyEvent(new MyParentCanGetBenevolenceEvent(_parentId, myamount, level+1));
                    }
                }
                if(level==2)//二度
                {
                    if (_role != UserRole.Consumer)
                    {
                        var myamount = Math.Round(amount * 0.025M,4);//我能获取的善心
                        ApplyEvent(new UserGetChildBenevolenceEvent(_walletId, myamount, level));
                    }
                }
            }
        }

        /// <summary>
        /// 接受新的店铺销售额（商品服务结束时，非用户下单时）
        /// </summary>
        /// <param name="sale"></param>
        /// <param name="surrenderPersent"></param>
        public void AcceptNewSale(decimal sale)
        {
            if(ConfigSettings.BenevolenceValue<=0)
            {
                throw new Exception("善心价值参数设置异常");
            }
            
            //计算我的推荐者的收益 商家销售额
            if(_parentId!=Guid.Empty)
            {
                var parentBenevolenceGetAmount = Math.Round((sale * ConfigSettings.RecommandStoreGetPercent / ConfigSettings.BenevolenceValue), 4);
                ApplyEvent(new UserGetChildStoreSaleBenevolenceEvent(_walletId, parentBenevolenceGetAmount));
            }
        }
        #endregion

        #region 善心联盟


        /// <summary>
        /// 设置用户为某个地区的联盟
        /// </summary>
        /// <param name="region"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <param name="level"></param>
        public void SetToPartner(string region,string province,string city,string county,PartnerLevel level)
        {
            ApplyEvent(new UserRoleToPartnerEvent(Id,_walletId,region,province,city,county,level));
        }
        /// <summary>
        /// 申请成为区域善心联盟 用户申请后台批复  后台可以批复单独设置某人为区域代理
        /// </summary>
        /// <param name="region">地区</param>
        /// <param name="level">基本 省级 市级 县级 </param>
        public void ApplyToRegionPartner(string region, PartnerLevel level)
        {
            //判断申请条件，符合申请条件提交申请
            //if(_storeId==Guid.Empty)
            //{//如果没有店铺就不符合条件
                if(_ambassadorExpireTime<DateTime.Now)
                {//大使身份过期
                    throw new Exception("不是店主身份或大使身份过期，无法申请联盟");
                }
            //}
            ApplyEvent(new RegionPartnerApplyedEvent(region,level));
        }

        #endregion

        #region 快递地址
        /// <summary>
        /// 添加快递地址
        /// </summary>
        /// <param name="expressAddress"></param>
        public void AddExpressAddress(ExpressAddressInfo expressAddressInfo)
        {
            ApplyEvent(new ExpressAddressAddedEvent(Guid.NewGuid(),expressAddressInfo));
        }

        /// <summary>
        /// 更新 快递地址
        /// </summary>
        /// <param name="expressAddressId"></param>
        /// <param name="expressAddressInfo"></param>
        public void UpdateExpressAddress(Guid expressAddressId, ExpressAddressInfo expressAddressInfo)
        {
            var expressaddress = _expressAddresses.SingleOrDefault(x => x.Id == expressAddressId);
            if (expressaddress == null)
            {
                throw new Exception("不存在该收货地址.");
            }
            ApplyEvent(new ExpressAddressUpdatedEvent(expressAddressId, expressAddressInfo));
        }

        /// <summary>
        /// 删除快递地址
        /// </summary>
        /// <param name="expressAddressId"></param>
        public void RemoveExpressAddress(Guid expressAddressId)
        {
            ApplyEvent(new ExpressAddressRemovedEvent(expressAddressId));
        }

        #endregion

        #region Event Handle Methods 通过事件更改聚合跟状态

        private void Handle(UserCreatedEvent evnt)
        {
            _info = evnt.Info;
            _parentId = evnt.ParentId;
            _expressAddresses = new List<ExpressAddress>();
            _userGifts = new List<UserGift>();
            _isLocked = false;
            _isFreeze = false;
            _myRecommends = new HashSet<Guid>();
            _role = UserRole.Consumer;
            _mySpending = 0;
            _walletId = evnt.WalletId;
            _cartId = evnt.CartId;
            _ambassadorExpireTime = DateTime.Now;
        }
        private void Handle(UserGetChildBenevolenceEvent evnt) { }
        private void Handle(MyParentCanGetBenevolenceEvent evnt) { }

        private void Handle(UserGetSaleBenevolenceEvent evnt) { }
        private void Handle(UserGetChildStoreSaleBenevolenceEvent evnt) { }

        private void Handle(UserRoleToPartnerEvent evnt) { }
        private void Handle(RegionPartnerApplyedEvent evnt) { }

        private void Handle(UserNickNameUpdatedEvent evnt)
        {
            _info.NickName = evnt.NickName;
        }

        private void Handle(UserEditedEvent evnt)
        {
            _info.NickName = evnt.NickName;
            _info.Gender = evnt.Gender;
        }

        private void Handle(UserGenderUpdatedEvent evnt)
        {
            _info.Gender = evnt.Gender;
        }
        private void Handle(UserRegionUpdatedEvent evnt)
        {
            _info.Region = evnt.Region;
        }
        private void Handle(UserPortraitUpdatedEvent evnt)
        {
            _info.Portrait = evnt.Portrait;
        }
      
        private void Handle(UserPasswordUpdatedEvent evnt)
        {
            _info.Password = evnt.Password;
        }

        private void Handle(UserLockedEvent evnt)
        {
            _isLocked = true;
        }

        private void Handle(UserUnLockedEvent evnt)
        {
            _isLocked = false;
        }

        private void Handle(UserFreezeEvent evnt)
        {
            _isFreeze = true;
        }

        private void Handle(UserUnFreezeEvent evnt)
        {
            _isFreeze = false;
        }
        

        private void Handle(ExpressAddressAddedEvent evnt)
        {
            _expressAddresses.Add(new ExpressAddress(evnt.ExpressAddressId,evnt.Info));
        }
        private void Handle(ExpressAddressUpdatedEvent evnt)
        {
            _expressAddresses.Single(x => x.Id == evnt.ExpressAddressId).Info = evnt.Info;
        }
        private void Handle(ExpressAddressRemovedEvent evnt)
        {
            _expressAddresses.Remove(_expressAddresses.Single(x => x.Id == evnt.ExpressAddressId));
        }

        private void Handle(UserGiftAddedEvent evnt)
        {
            _userGifts.Add(new UserGift(evnt.UserGiftId,evnt.GiftInfo,evnt.ExpressAddressInfo,evnt.Remark));
        }
        private void Handle(UserGiftPayedEvent evnt)
        {
            _userGifts.Single(x => x.Id == evnt.UserGiftId).Remark = "未发货";
        }

        private void Handle(UserGiftRemarkChangedEvent evnt)
        {
            _userGifts.Single(x => x.Id == evnt.UserGiftId).Remark = evnt.Remark;
        }


        private void Handle(UserRoleToPasserEvent evnt)
        {
            _role = UserRole.Passer;
        }

        private void Handle(UserNewSpendingEvent evnt)
        {
            _mySpending += evnt.Amount;
        }
        private void Handle(UserRoleToAmbassadorEvent evnt)
        {
            if (!evnt.OnlyUpdateTime)
            {
                _role = UserRole.Ambassador;
            }
            _ambassadorExpireTime = evnt.ExpireTime;
        }
        
        private void Handle(UserSpendingTransformToBenevolenceEvent evnt)
        {
        }

        #endregion
    }

    
}
