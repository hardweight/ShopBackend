using System.ComponentModel;

namespace Shop.Common.Enums
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public enum UserRole
    {
        [Description("全部")]
        All=-1,
        [Description("善心使者")]
        Consumer = 0,

        [Description("传递使者")]
        Passer = 1,

        [Description("店主")]
        Ambassador = 2,

        [Description("区域合伙人")]
        RegionPartner = 3,

        [Description("行业合伙人")]
        SectionPartner = 4
    }

    
    public enum UserLock
    {
        /// <summary>
        /// 未锁定
        /// </summary>
        [Description("未锁定")]
        UnLocked=0,
        /// <summary>
        /// 锁定
        /// </summary>
        [Description("锁定")]
        Locked=1
    }

    public enum UserFreeze
    {
        [Description("未冻结")]
        UnFreeze=0,
        [Description("冻结")]
        Freeze=1
    }
}
