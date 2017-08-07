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
                if (_walletStatisticInfo == null)
                {
                    ApplyEvent(new WalletStatisticInfoChangedEvent(new WalletStatisticInfo(
                        0,
                        0,
                        0,
                       0,
                       0)));
                }
                else if(cashTransfer.GetTransferType()==CashTransferType.Incentive)
                {
                    //善心激励
                    ApplyEvent(new WalletStatisticInfoChangedEvent(new WalletStatisticInfo(
                        cashTransferInfo.Amount,
                        _walletStatisticInfo.Earnings+cashTransferInfo.Amount,
                        _walletStatisticInfo.YesterdayIndex,
                        _walletStatisticInfo.BenevolenceTotal,
                        _walletStatisticInfo.TodayBenevolenceAdded
                        )));
                }

                finallyValue += cashTransferInfo.Amount;
            }
            

            //新的记录接受后，发出新记录接受的事件
            ApplyEvent(new NewCashTransferEvent(_userId,cashTransfer.Id, finallyValue));
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
            if (benevolenceTransferInfo.Direction == WalletDirection.Out)
            {//如果是出账 判断账号余额是否够
                if (_benevolence < benevolenceTransferInfo.Amount)
                {
                    throw new Exception("账户余额不足");
                }
                finallyValue -= benevolenceTransferInfo.Amount;
            }
            else
            {//如果是进账 更新统计信息
                
                if (_walletStatisticInfo == null)
                {
                    ApplyEvent(new WalletStatisticInfoChangedEvent(new WalletStatisticInfo(
                        0,
                        0,
                        0,
                       benevolenceTransferInfo.Amount,
                       0)));
                }
                else
                {
                    ApplyEvent(new WalletStatisticInfoChangedEvent(new WalletStatisticInfo(
                        _walletStatisticInfo.YesterdayEarnings,
                        _walletStatisticInfo.Earnings,
                        _walletStatisticInfo.YesterdayIndex,
                        _walletStatisticInfo.BenevolenceTotal + benevolenceTransferInfo.Amount,
                        _walletStatisticInfo.TodayBenevolenceAdded
                        )));
                }
                finallyValue += benevolenceTransferInfo.Amount;
            }
            
            ApplyEvent(new NewBenevolenceTransferEvent(_userId, benevolenceTransfer.Id,finallyValue));
        }

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
            if(_benevolence>1)
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
       

        #region Handler
        private void Handle(WalletCreatedEvent evnt)
        {
            _cash = 0;
            _benevolence = 0;
            _bankCards = new List<BankCard>();
            _cashTransfers = new HashSet<Guid>();
            _benevolenceTransfers = new HashSet<Guid>();
            _userId = evnt.UserId;
        }

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

        private void Handle(NewCashTransferEvent evnt)
        {
            _cash = evnt.FinallyValue;
        }
        private void Handle(NewBenevolenceTransferEvent evnt)
        {
            _benevolence = evnt.FinallyValue;
        }

        private void Handle(WalletStatisticInfoChangedEvent evnt)
        {
            _walletStatisticInfo = evnt.StatisticInfo;
        }
        #endregion
    }
}
