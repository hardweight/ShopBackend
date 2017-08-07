using System;
using ENode.Domain;
using System.Collections.Generic;
using Shop.Domain.Events.Users;
using Shop.Common;
using Xia.Common.Extensions;
using System.Linq;
using Shop.Domain.Models.Partners;
using Shop.Domain.Events.Users.ExpressAddresses;
using Shop.Domain.Models.Users.UserGifts;
using Shop.Domain.Events.Users.UserGifts;
using Shop.Domain.Models.Users.ExpressAddresses;

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
        private Guid _walletId;//用户钱包ID 怎样获取？
        private Guid _storeId;//我的店铺ID  怎样获取？
        private bool _isLocked;//是否锁定账号 只用于限制登陆
        private bool _isFreeze;//是否冻结账号 怀疑账号被盗可以冻结账号
        private UserRole _role;//用户角色
        private ISet<Guid> _myRecommends;//保存我推荐的用户
        private decimal _mySpending;//我的消费额
        private decimal _unTransformSpending;//未转换为善心的消费额
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
            ApplyEvent(new UserCreatedEvent(info, parent == null ? Guid.Empty : parent.Id));
        }

        #region 基本信息修改
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
                throw new Exception("20");
            }
            ApplyEvent(new UserNickNameUpdatedEvent(nickName));
        }

        


        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="password"></param>
        public void UpdatePassword(string password)
        {
            password.CheckNotNullOrEmpty(nameof(password));
            if (password.Length < 8)
            {
                throw new Exception("密码长度不能小于8");
            }
            if (password.Length > 20)
            {
                throw new Exception("密码长度不能超过20");
            }
            ApplyEvent(new UserPasswordUpdatedEvent(password));
        }

        /// <summary>
        /// 更新性别
        /// </summary>
        /// <param name="gender"></param>
        public void UpdateGender(string gender)
        {
            gender.CheckNotNullOrEmpty(nameof(gender));
            if (!"男,女".IsIncludeItem(gender))
            {
                throw new Exception("只接受参数值：男/女");
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
                throw new Exception("user already locked.");
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
                throw new Exception("user already unlocked.");
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
                throw new Exception("user already Freeze.");
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
                throw new Exception("user already unFreeze.");
            }
            ApplyEvent(new UserUnFreezeEvent());
        }
        
        #endregion

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
        /// <summary>
        /// 接受自己新的消费额 我的订单完成时
        /// </summary>
        /// <param name="amount">订单额</param>
        /// <param name="surrenderPersent">商品的让利比例</param>
        public void AcceptMyNewSpending(decimal amount,decimal surrenderPersent)
        {
            if (amount <= 0) return;
            ApplyEvent(new UserNewSpendingEvent(amount));

            //用户角色升级
            if(_mySpending>=ConfigSettings.ToPasserSpendingAmount && _role==UserRole.Consumer)
            {
                //更新用户级别为传递使者
                ApplyEvent(new UserRoleToPasserEvent());
            }

            //用户消费满100转换为善心
            if (ConfigSettings.BenevolenceValue <= 0)
            {
                throw new Exception("善心价值配置异常");
            }
            var benevolenceAmount = Math.Floor(_unTransformSpending / ConfigSettings.BenevolenceValue);//可转换量
            var leftUnTransformAmount = _unTransformSpending % ConfigSettings.BenevolenceValue;//剩余未转换量
            if (benevolenceAmount>=1)
            {
                //大于一个善心才转换
                ApplyEvent(new UserSpendingTransformToBenevolenceEvent(_walletId, benevolenceAmount,leftUnTransformAmount));
            }
            

            //是否可以享受消费激励 用户5倍 直接激励
            if(_role!=UserRole.Consumer)
            {
                ///消费者都是5倍让利
                var consumerBenevolence = Math.Round((amount * surrenderPersent * ConfigSettings.ConsumerMultiple / ConfigSettings.BenevolenceValue), 4);
                ApplyEvent(new UserGetSpendingBenevolenceEvent(_walletId, consumerBenevolence));
            }

            //计算我的推荐者的 间接激励
            if(_parentId!=Guid.Empty)
            {//如果我有推荐者，将我消费的信息广播给我的推荐者推荐者自己计算自己的一度或二度激励
                var consumerBenevolence = Math.Round((amount * surrenderPersent * ConfigSettings.ConsumerMultiple / ConfigSettings.BenevolenceValue), 4);
                ApplyEvent(new MyParentCanGetBenevolenceEvent(_parentId, consumerBenevolence,1));
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
            if(level>0 && level<=2)
            {//目前只接受一度二度奖励
                if (level==1)
                {//一度
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
                if(level==2)
                {//二度
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
        public void AcceptNewSale(decimal sale, decimal surrenderPersent)
        {
            if(ConfigSettings.BenevolenceValue<=0)
            {
                throw new Exception("善心价值参数设置异常");
            }
            var storeBenevolence = 0M;
            if (surrenderPersent >= 0.15M)
            {//让利超过15% 商家双倍让利
                storeBenevolence = Math.Round((sale * surrenderPersent * ConfigSettings.StoreMultiple / ConfigSettings.BenevolenceValue), 4);
            }
            else
            {//不超过15% 商家单倍让利
                storeBenevolence = Math.Round((sale * surrenderPersent / ConfigSettings.BenevolenceValue), 4);
            }
            //认同公益事业并愿意为公益做贡献的商家，销售产品即可获得爱心激励
            ApplyEvent(new UserGetSaleBenevolenceEvent(_walletId,storeBenevolence));

            //计算我的推荐者的收益 商家销售额
            if(_parentId!=Guid.Empty)
            {
                var parentBenevolenceGetAmount = Math.Round((sale * ConfigSettings.RecommandStoreGetPercent / ConfigSettings.BenevolenceValue), 4);
                ApplyEvent(new UserGetChildStoreSaleBenevolenceEvent(_walletId, parentBenevolenceGetAmount));
            }
        }
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
            if(_storeId==Guid.Empty)
            {//如果没有店铺就不符合条件
                if(_ambassadorExpireTime<DateTime.Now)
                {//大使身份过期
                    throw new Exception("不是店主身份或大使身份过期，无法申请联盟");
                }
            }
            ApplyEvent(new RegionPartnerApplyedEvent(region,level));
        }

        

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
            _ambassadorExpireTime = DateTime.Now;
        }

        private void Handle(UserNickNameUpdatedEvent evnt)
        {
            _info.NickName = evnt.NickName;
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
            _unTransformSpending += evnt.Amount;
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
            _unTransformSpending = evnt.LeftUnTransformAmount;
        }

        #endregion
    }

    
}
