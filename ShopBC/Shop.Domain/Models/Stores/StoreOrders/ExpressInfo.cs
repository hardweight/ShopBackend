namespace Shop.Domain.Models.Stores.StoreOrders
{
    public class ExpressInfo
    {
        public string ExpressName { get;private set; }
        public string ExpressNumber { get;private set; }

        public ExpressInfo(string expressName,string expressNumber)
        {
            ExpressName = expressName;
            ExpressNumber = expressNumber;
        }
    }
}
