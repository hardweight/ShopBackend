using ECommon.Components;
using ECommon.Dapper;
using Shop.Common;
using Shop.ReadModel.Wallets.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Shop.ReadModel.Wallets
{
    /// <summary>
    /// Q端查询服务 Dapper
    /// </summary>
    [Component]
    public class WalletQueryService: BaseQueryService,IWalletQueryService
    {
        /// <summary>
        /// 钱包信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Wallet Info(Guid id)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Wallet>(new { Id = id }, ConfigSettings.WalletTable).SingleOrDefault();
            }
        }

        /// <summary>
        /// 待分配善心量
        /// </summary>
        /// <returns></returns>
        public decimal TotalBenevolence()
        {
            //var sql = string.Format(@"select sum(Benevolence) as totalBenevolence from {0}", ConfigSettings.WalletTable);

            //using (var connection = GetConnection())
            //{
            //    return connection.Query<decimal>(sql).FirstOrDefault();
            //}
            using (var connection = GetConnection())
            {
                var stores = connection.QueryList<Wallet>(new { }, ConfigSettings.WalletTable);
                return stores.Sum(x => x.Benevolence);
            }
        }


        /// <summary>
        /// 钱包信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //public Wallet InfoByUserId(Guid userId)
        //{
        //    using (var connection = GetConnection())
        //    {
        //        return connection.QueryList<Wallet>(new { UserId = userId }, ConfigSettings.WalletTable).SingleOrDefault();
        //    }
        //}

        /// <summary>
        /// 获取用户现金记录
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        public IEnumerable<CashTransfer> GetCashTransfers(Guid walletId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<CashTransfer>(new { WalletId = walletId }, ConfigSettings.CashTransferTable).ToList();
            }
        }
        /// <summary>
        /// 获取用户善心记录
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        public IEnumerable<BenevolenceTransfer> GetBenevolenceTransfers(Guid walletId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<BenevolenceTransfer>(new { WalletId = walletId }, ConfigSettings.BenevolenceTransferTable);
            }
        }

        public IEnumerable<Wallet> ListPage()
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Wallet>(new {  }, ConfigSettings.WalletTable);
            }
        }


        public IEnumerable<WithdrawApply> WithdrawApplyLogs(Guid walletId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<WithdrawApply>(new { WalletId = walletId }, ConfigSettings.WithdrawApplysTable);
            }
        }
        public IEnumerable<WithdrawApply> WithdrawApplyLogs()
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<WithdrawApply>(new { }, ConfigSettings.WithdrawApplysTable);
            }
        }

        public IEnumerable<RechargeApply> RechargeApplyLogs(Guid walletId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<RechargeApply>(new { WalletId = walletId }, ConfigSettings.RechargeApplysTable);
            }
        }
        public IEnumerable<RechargeApply> RechargeApplyLogs()
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<RechargeApply>(new { }, ConfigSettings.RechargeApplysTable);
            }
        }

        /// <summary>
        /// 获取银行卡
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        public IEnumerable<BankCard> GetBankCards(Guid walletId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<BankCard>(new { WalletId = walletId }, ConfigSettings.BankCardTable).ToList();
            }
        }
    }
}
