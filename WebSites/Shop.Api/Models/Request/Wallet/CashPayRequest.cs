namespace Shop.Api.Models.Request.Wallet
{
    public class CashPayRequest
    {
        public string OrderNumber { get; set; }
        public decimal Amount { get; set; }
        public string AccessCode { get; set; }
    }
}