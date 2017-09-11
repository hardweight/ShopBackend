using Shop.Common.Enums;
using Shop.ReadModel.Wallets.Dtos;
using System;
using System.Collections.Generic;

namespace Shop.ReadModel.Wallets
{
    public interface IWalletQueryService
    {
        Wallet Info(Guid Id);

        decimal TotalBenevolence();

        IEnumerable<Wallet> ListPage();
        IEnumerable<Wallet> AllWallets();


        IEnumerable<WithdrawApply> WithdrawApplyLogs(Guid walletId);
        IEnumerable<WithdrawApply> WithdrawApplyLogs();

        IEnumerable<RechargeApply> RechargeApplyLogs(Guid walletId);
        IEnumerable<RechargeApply> RechargeApplyLogs();


        IEnumerable<BankCard> GetBankCards(Guid walletId);

        IEnumerable<CashTransfer> GetCashTransfers(Guid walletId);
        IEnumerable<CashTransfer> GetCashTransfers(Guid walletId, CashTransferType type);

        IEnumerable<BenevolenceTransfer> GetBenevolenceTransfers(Guid walletId);
        IEnumerable<BenevolenceTransfer> GetBenevolenceTransfers(Guid walletId, BenevolenceTransferType type);

    }
}
