using System;

namespace Shop.Api.ViewModels.Wallet
{
    public class BankCardViewModel
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public string BankName { get; set; }
        public string OwnerName { get; set; }
        public string Number { get; set; }
    }
}