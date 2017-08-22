namespace Shop.Domain.Models.Stores.StoreOrders
{
    public class RefoundApplyInfo
    {
        public string Reason { get;private set; }
        public decimal RefundAmount { get;private  set; }

        public RefoundApplyInfo(string reason,decimal refundAmount)
        {
            Reason = reason;
            RefundAmount = refundAmount;
        }
    }
}
