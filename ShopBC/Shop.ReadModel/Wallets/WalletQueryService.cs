using ECommon.Components;
using ECommon.Dapper;
using Shop.Common;
using Shop.ReadModel.Wallets.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="walletId"></param>
        /// <returns></returns>
        public Wallet Info(Guid walletId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Wallet>(new { Id = walletId }, ConfigSettings.WalletTable).SingleOrDefault();
            }
        }

        /// <summary>
        /// 钱包信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Wallet InfoByUserId(Guid userId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Wallet>(new { UserId = userId }, ConfigSettings.WalletTable).SingleOrDefault();
            }
        }

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
                return connection.QueryList<BenevolenceTransfer>(new { WalletId = walletId }, ConfigSettings.BenevolenceTransferTable).ToList();
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
