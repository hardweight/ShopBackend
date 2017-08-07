using System.ComponentModel;

namespace Shop.Domain.Models.Users
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public enum UserRole
    {
        [Description("善心使者")]
        Consumer = 0,

        [Description("传递使者")]
        Passer = 1,

        [Description("传递大使")]
        Ambassador = 2,

        [Description("区域合伙人")]
        RegionPartner = 3,

        [Description("行业合伙人")]
        SectionPartner = 4
    }

    
}
