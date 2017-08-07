using System;

namespace Shop.Domain.Models.Users.ExpressAddresses
{
    /// <summary>
    /// 快递地址 信息
    /// </summary>
    [Serializable]
    public class ExpressAddressInfo
    {
        public string Name { get; private set; }
        public string Mobile { get;private  set; }
        public string Region { get; private set; }
        public string Address { get; private set; }
        public string Zip { get; private set; }

        public ExpressAddressInfo(string name,string mobile,string region,string address,string zip)
        {
            Name = name;
            Region = region;
            Mobile = mobile;
            Address = address;
            Zip = zip;
        }
    }
}
