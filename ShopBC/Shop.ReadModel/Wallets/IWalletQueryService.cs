using Shop.ReadModel.Wallets.Dtos;
using System;
using System.Collections.Generic;

namespace Shop.ReadModel.Wallets
{
    public interface IWalletQueryService
    {
        Wallet Info(Guid userId);
        Wallet InfoByUserId(Guid userId);


        IEnumerable<BankCard> GetBankCards(Guid walletId);
        IEnumerable<CashTransfer> GetCashTransfers(Guid walletId);
        IEnumerable<BenevolenceTransfer> GetBenevolenceTransfers(Guid walletId);
    }
}
