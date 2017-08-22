using Shop.Api.ViewModels.Wallet;
using Shop.Commands.Wallets.BankCards;
using System;
using Dtos = Shop.ReadModel.Wallets.Dtos;

namespace Shop.Api.Extensions
{
    public static class WalletViewModelWalletExtension
    {
        public static WalletViewModel ToWalletModel(this Dtos.Wallet value)
        {
            return new WalletViewModel()
            {
                Id=value.Id,
                UserId=value.UserId,
                AccessCode=value.AccessCode,
                Cash=value.Cash,
                Benevolence=value.Benevolence,
                BenevolenceTotal=value.BenevolenceTotal,
                Earnings=value.Earnings,
                TodayBenevolenceAdded=value.TodayBenevolenceAdded,
                YesterdayEarnings=value.YesterdayEarnings,
                YesterdayIndex=value.YesterdayIndex,
            };
        }

        public static AddBankCardCommand ToAddBankCardCommand(this BankCardViewModel bankcardViewModel)
        {
            var command = new AddBankCardCommand(
                bankcardViewModel.WalletId,
                bankcardViewModel.BankName,
                bankcardViewModel.OwnerName,
                bankcardViewModel.Number);

            return command;
        }
        
    }
}