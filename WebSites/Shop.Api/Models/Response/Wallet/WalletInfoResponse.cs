using System;

namespace Shop.Api.Models.Response.Wallet
{
    public class WalletInfoResponse:BaseApiResponse
    {
        public WalletInfo WalletInfo { get; set; }
    }
    public class WalletInfo
    {
        public Guid Id { get; set; }
        public string AccessCode { get; set; }
        public decimal Cash { get; set; }
        public decimal Benevolence { get; set; }
        public decimal YesterdayEarnings { get; set; }
        public decimal Earnings { get; set; }
    }
}