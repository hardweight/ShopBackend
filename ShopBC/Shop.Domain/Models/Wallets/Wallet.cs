using ENode.Domain;
using Shop.Domain.Events.Wallets.BankCards;
using Shop.Domain.Events.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using Xia.Common.Extensions;
using Shop.Domain.Models.Wallets.CashTransfers;
using Shop.Domain.Models.Wallets.BankCards;
using Shop.Domain.Models.Wallets.BenevolenceTransfers;
using Shop.Domain.Models.Wallets.WithdrawApplys;
using Shop.Domain.Events.Wallets.WithdrawApplys;
using Shop.Common.Enums;
using Shop.Domain.Models.Wallets.RechargeApplys;
using Shop.Domain.Events.Wallets.RechargeApplys;

namespace Shop.Domain.Models.Wallets
{
    /// <summary>
    /// 用户钱包 聚合跟
    /// </summary>
    public class Wallet:AggregateRoot<Guid>
    {
        private Guid _userId;//所有人
        private decimal _cash;//现金 量
        private decimal _benevolence;//善心 量

        private string _accessCode;//访问密码 可以理解为交易密码

        private ISet<Guid> _cashTransfers;//现金账记录
        private ISet<Guid> _benevolenceTransfers;//善心账记录
        private IList<BankCard> _bankCards;//银行卡
        private IList<WithdrawApply> _withdrawApplys;//提现记录
        private IList<RechargeApply> _rechargeApplys;//线下充值记录

        private WalletStatisticInfo _walletStatisticInfo;//钱包统计信息

        public Wallet(Guid id,Guid userId):base(id)
        {
            ApplyEvent(new WalletCreatedEvent(userId));
        }

        /// <summary>
        /// 设置支付密码
        /// </summary>
        /// <param name="accessCode"></param>
        public void SetAccessCode(string accessCode)
        {
            if(!accessCode.IsPayPassword())
            {
                throw new Exception("支付密码为6为纯数字");
            }
            ApplyEvent(new WalletAccessCodeUpdatedEvent(accessCode));
        }

        #region 银行卡
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="bankCardInfo"></param>
        public void AddBankCard(BankCardInfo bankCardInfo)
        {
            ApplyEvent(new BankCardAddedEvent(Guid.NewGuid(), bankCardInfo));
        }

        /// <summary>
        /// 更新 
        /// </summary>
        /// <param name="bankCardId"></param>
        /// <param name="bankCardInfo"></param>
        public void UpdateBankCard(Guid bankCardId, BankCardInfo bankCardInfo)
        {
            var bankcard = _bankCards.SingleOrDefault(x => x.Id == bankCardId);
            if (bankcard == null)
            {
                throw new Exception("不存在该银行卡.");
            }
            ApplyEvent(new BankCardUpdatedEvent(bankCardId, bankCardInfo));
        }

        /// <summary>
        ///  删除
        /// </summary>
        /// <param name="bankCardId"></param>
        public void RemoveBankCard(Guid bankCardId)
        {
            ApplyEvent(new BankCardRemovedEvent(bankCardId));
        }

        #endregion

        #region 处理流水信息

        
        /// <summary>
        /// 接受新的现金记录   更新统计信息
        /// </summary>
        /// <param name="reply"></param>
        public void AcceptNewCashTransfer(CashTransfer cashTransfer)
        {
            if (!_cashTransfers.Add(cashTransfer.Id)) return;

            var finallyValue = _cash;
            //业务逻辑判断
            CashTransferInfo cashTransferInfo = cashTransfer.GetInfo();
            //统计信息
            var walletStatisticInfo = _walletStatisticInfo;

            if(cashTransferInfo.Direction==WalletDirection.Out)
            {
                //如果是出账 判断账号余额是否够
                if(_cash<cashTransferInfo.Amount)
                {
                    throw new Exception("账户余额不足");
                }
                finallyValue -= cashTransferInfo.Amount;
            }
            else
            {
                //进账 判断收益
                if(cashTransfer.GetTransferType() == CashTransferType.Incentive)
                {
                    //善心激励
                    if (_walletStatisticInfo.UpdatedOn.Date.Equals(DateTime.Now.Date))
                    {
                        //非第一个
                        walletStatisticInfo.YesterdayEarnings += cashTransferInfo.Amount;
                        walletStatisticInfo.Earnings += cashTransferInfo.Amount;
                        walletStatisticInfo.UpdatedOn = DateTime.Now;
                    }
                    else
                    {
                        //今日第一个
                        walletStatisticInfo.YesterdayEarnings = cashTransferInfo.Amount;
                        walletStatisticInfo.Earnings += cashTransferInfo.Amount;
                        walletStatisticInfo.UpdatedOn = DateTime.Now;
                    }
                }
                finallyValue += cashTransferInfo.Amount;
            }
            //新的记录接受后，发出新记录接受的事件
            ApplyEvent(new NewCashTransferAcceptedEvent(_userId,cashTransfer.Id, finallyValue,walletStatisticInfo));
        }

        /// <summary>
        /// 接受新的善心记录 更新统计信息
        /// </summary>
        /// <param name="benevolenceTransfer"></param>
        public void AcceptNewBenevolenceTransfer(BenevolenceTransfer benevolenceTransfer)
        {
            if (!_benevolenceTransfers.Add(benevolenceTransfer.Id)) return;

            var finallyValue = _benevolence;
            //业务逻辑判断
            BenevolenceTransferInfo benevolenceTransferInfo = benevolenceTransfer.GetInfo();
            //统计信息
            var walletStatisticInfo = _walletStatisticInfo;

            if (benevolenceTransferInfo.Direction == WalletDirection.Out)
            {//如果是出账 判断账号余额是否够
                if (_benevolence < benevolenceTransferInfo.Amount)
                {
                    throw new Exception("账户余额不足");
                }
                finallyValue -= benevolenceTransferInfo.Amount;
            }
            else
            {
                //如果是进账 更新统计信息
                if(_walletStatisticInfo.UpdatedOn.Date.Equals(DateTime.Now.Date))
                {
                    //今日非第一个记录
                    walletStatisticInfo.BenevolenceTotal += benevolenceTransferInfo.Amount;
                    walletStatisticInfo.TodayBenevolenceAdded += benevolenceTransferInfo.Amount;
                    walletStatisticInfo.UpdatedOn = DateTime.Now;
                }
               else
                {
                    //今日第一个收入
                    walletStatisticInfo.BenevolenceTotal += benevolenceTransferInfo.Amount;
                    walletStatisticInfo.TodayBenevolenceAdded = benevolenceTransferInfo.Amount;
                    walletStatisticInfo.UpdatedOn = DateTime.Now;
                }
                finallyValue += benevolenceTransferInfo.Amount;
            }
            
            ApplyEvent(new NewBenevolenceTransferAcceptedEvent(_userId, benevolenceTransfer.Id,finallyValue,walletStatisticInfo));
        }
        #endregion
        /// <summary>
        /// 激励善心 计算收益 并减扣善心
        /// </summary>
        /// <param name="benevolenceIndex"></param>
        public void IncentiveBenevolence(decimal benevolenceIndex)
        {
            if(benevolenceIndex<=0 || benevolenceIndex>1)
            {
                throw new Exception("善心指数异常");
            }
            if(_benevolence>=1)
            {
                //只有善心值大于1时才参与激励

                //激励善心就是善心值转换为现金而已，创建两个激励类型的记录即可 这个工作交给ProcessManager来做
                //MessagePublisher 将用户激励信息发布出去 ProcessManager接受消息处理
                
                var benevolenceValue = _benevolence * 100; //我的善心价值
                var incentiveValue = benevolenceValue * benevolenceIndex;//本次激励收益
                var benevolenceDeduct = _benevolence * benevolenceIndex;//善心扣除数量
                ApplyEvent(new IncentiveUserBenevolenceEvent(_userId,benevolenceIndex,incentiveValue,benevolenceDeduct));
            }
        }

        #region 提现申请

        
        /// <summary>
        /// 提交提现申请
        /// </summary>
        /// <param name="withdrawApplyId"></param>
        /// <param name="info"></param>
        public void ApplyWithdraw(Guid withdrawApplyId,WithdrawApplyInfo info)
        {
            info.CheckNotNull(nameof(info));
            ApplyEvent(new WithdrawApplyCreatedEvent(withdrawApplyId,info,WithdrawApplyStatus.Placed));
        }

        /// <summary>
        /// 审核提现状态
        /// </summary>
        /// <param name="withdrawApplyId"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        public void ChangeWithdrawApplyStatus(Guid withdrawApplyId,WithdrawApplyStatus status,string remark)
        {
            if(status==WithdrawApplyStatus.Success)
            {
                
                var withdrawApply = _withdrawApplys.SingleOrDefault(x => x.Id == withdrawApplyId);
                if(withdrawApply==null)
                {
                    throw new Exception("不存在该提现申请.");
                }
                if(_cash< withdrawApply.Info.Amount)
                {//判断钱包钱包余额是否够提现金额
                    throw new Exception("当期余额不足，无法提现");
                }
                ApplyEvent(new WithdrawApplySuccessEvent(withdrawApply.Info.Amount));
            }
            ApplyEvent(new WithdrawApplyStatusChangedEvent(withdrawApplyId, status, remark));
        }
        #endregion

        #region 线下充值
        /// <summary>
        /// 提交充值申请
        /// </summary>
        /// <param name="rechargeApplyId"></param>
        /// <param name="info"></param>
        public void ApplyRecharge(Guid rechargeApplyId, RechargeApplyInfo info)
        {
            info.CheckNotNull(nameof(info));
            ApplyEvent(new RechargeApplyCreatedEvent(rechargeApplyId, info, RechargeApplyStatus.Placed));
        }

        public void ChangeRechargeApplyStatus(Guid rechargeApplyId, RechargeApplyStatus status, string remark)
        {
            if (status == RechargeApplyStatus.Success)
            {

                var rechargeApply = _rechargeApplys.SingleOrDefault(x => x.Id == rechargeApplyId);
                if (rechargeApply == null)
                {
                    throw new Exception("不存在该充值申请.");
                }
               
                ApplyEvent(new RechargeApplySuccessEvent(rechargeApply.Info.Amount));
            }
            ApplyEvent(new RechargeApplyStatusChangedEvent(rechargeApplyId, status, remark));
        }


        #endregion

        #region Handler
        private void Handle(WalletCreatedEvent evnt)
        {
            _cash = 0;
            _benevolence = 0;
            _bankCards = new List<BankCard>();
            _withdrawApplys = new List<WithdrawApply>();
            _rechargeApplys = new List<RechargeApply>();
            _cashTransfers = new HashSet<Guid>();
            _benevolenceTransfers = new HashSet<Guid>();
            _userId = evnt.UserId;
            _walletStatisticInfo = new WalletStatisticInfo(0, 0, 0, 0, 0, DateTime.Now);
        }

        //提现申请
        private void Handle(WithdrawApplyCreatedEvent evnt)
        {
            _withdrawApplys.Add(new WithdrawApply(evnt.WithdrawApplyId,evnt.Info,evnt.Status));
        }
        private void Handle(WithdrawApplyStatusChangedEvent evnt)
        {
            _withdrawApplys.Single(x => x.Id == evnt.WithdrawApplyId).Status = evnt.Status;
            _withdrawApplys.Single(x => x.Id == evnt.WithdrawApplyId).Info.Remark = evnt.Remark;
        }
        private void Handle(WithdrawApplySuccessEvent evnt) { }
        private void Handle(IncentiveUserBenevolenceEvent evnt) { }
        //充值申请
        private void Handle(RechargeApplyCreatedEvent evnt)
        {
            _rechargeApplys.Add(new RechargeApply(evnt.RechargeApplyId, evnt.Info, evnt.Status));
        }
        private void Handle(RechargeApplyStatusChangedEvent evnt)
        {
            _rechargeApplys.Single(x => x.Id == evnt.RechargeApplyId).Status = evnt.Status;
            _rechargeApplys.Single(x => x.Id == evnt.RechargeApplyId).Info.Remark = evnt.Remark;
        }

        private void Handle(RechargeApplySuccessEvent evnt) { }





        private void Handle(WalletAccessCodeUpdatedEvent evnt)
        {
            _accessCode = evnt.AccessCode;
        }
        private void Handle(BankCardAddedEvent evnt)
        {
            _bankCards.Add(new BankCard(evnt.BankCardId,evnt.Info));
        }
        private void Handle(BankCardUpdatedEvent evnt)
        {
            _bankCards.Single(x => x.Id == evnt.BankCardId).Info = evnt.Info;
        }
        private void Handle(BankCardRemovedEvent evnt)
        {
            _bankCards.Remove(_bankCards.Single(x => x.Id == evnt.BankCardId));
        }

        private void Handle(NewCashTransferAcceptedEvent evnt)
        {
            _cash = evnt.FinallyValue;
            _walletStatisticInfo = evnt.StatisticInfo;
        }
        private void Handle(NewBenevolenceTransferAcceptedEvent evnt)
        {
            _benevolence = evnt.FinallyValue;
            _walletStatisticInfo = evnt.StatisticInfo;
        }

       
        #endregion
    }
}
