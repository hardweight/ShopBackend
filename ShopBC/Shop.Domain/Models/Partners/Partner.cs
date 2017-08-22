using ENode.Domain;
using Shop.Common;
using Shop.Common.Enums;
using Shop.Domain.Events.Partners;
using System;

namespace Shop.Domain.Models.Partners
{
    /// <summary>
    /// 联盟 聚合跟  是否结算是联盟用户决定的不要系统设置
    /// </summary>
    public class Partner:AggregateRoot<Guid>
    {
        private Guid _userId;//联盟关联用户
        private Guid _walletId;//联盟用户的钱包
        private string _region;//地区名称
        private string _province;//省,怎样获取聚合跟的省市县
        private string _city;//市
        private string _county;//县
        private PartnerLevel _level;//级别
        private decimal _unBalanceAmount;//未结算数量

        public Partner(Guid id,Guid userId,Guid walletId,string region,string province,string city,string county,PartnerLevel level):base(id)
        {
            ApplyEvent(new PartnerCreatedEvent(userId,walletId,region,province,city,county, level));
        }

        /// <summary>
        /// 申请结算 用户申请结算
        /// </summary>
        public void ApplyBalance()
        {
            var amountPersent = 0M;
            switch(_level)
            {
                case PartnerLevel.Province:
                    amountPersent = 0.005M;
                    break;
                case PartnerLevel.City:
                    amountPersent = 0.005M;
                    break;
                case PartnerLevel.County:
                    amountPersent = 0.01M;
                    break;
            }
            
            ApplyEvent(new ApplyBalancedEvent());
            //获得的善心量
            var myBenevolenceAmount = Math.Round((_unBalanceAmount * amountPersent / ConfigSettings.BenevolenceValue), 4);
            //用户应该获得联盟善心结算量
            ApplyEvent(new PartnerShouldGetBenevolenceEvent(_userId,_walletId,myBenevolenceAmount));
        }

        /// <summary>
        /// 接受新的地区销售
        /// </summary>
        /// <param name="amount"></param>
        public void AcceptNewSale(string region,decimal amount)
        {
            if (_region.Equals(region))
            {
                //是自己的地区，接受新的销售额
                var atlastAmount = _unBalanceAmount + amount;
                ApplyEvent(new AcceptedNewSaleEvent(atlastAmount));

                if(_level==PartnerLevel.County)
                {
                    //如果当前统计为县区，则要向上递归联盟统计
                    ApplyEvent(new ParentPartnerShouldAcceptNewSaleEvent(_city,amount,PartnerLevel.City));
                }
                if(_level==PartnerLevel.City)
                {
                    ApplyEvent(new ParentPartnerShouldAcceptNewSaleEvent(_province,amount, PartnerLevel.Province));
                }
            }
        }

        #region Handler
        private void Handle(PartnerCreatedEvent evnt)
        {
            _userId = evnt.UserId;
            _walletId = evnt.WalletId;
            _region = evnt.Region;
            _province = evnt.Province;
            _city = evnt.City;
            _county = evnt.County;
            _level = evnt.Level;
        }
        private void Handle(ApplyBalancedEvent evnt)
        {
            _unBalanceAmount = 0;
        }
        private void Handle(AcceptedNewSaleEvent evnt)
        {
            _unBalanceAmount = evnt.Amount;
        }
        #endregion

    }
}
