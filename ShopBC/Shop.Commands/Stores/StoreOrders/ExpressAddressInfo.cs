namespace Shop.Commands.Stores.StoreOrders
{
    public class ExpressAddressInfo
    {
        public string Region { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Zip { get; set; }

        public ExpressAddressInfo(
            string region,
            string address,
            string name,
            string mobile,
            string zip)
        {
            Region = region;
            Address = address;
            Name = name;
            Mobile = mobile;
            Zip = zip;
        }
    }
}
