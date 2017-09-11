namespace Shop.Api.Models.Request.Payment
{
    public class PaymentRequest
    {
        public int Amount { get; set; }
        public string OrderNumber { get; set; }
    }
}