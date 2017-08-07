using System;
using System.ComponentModel;

namespace Shop.ReadModel.Users.Dtos
{
    /// <summary>
    /// 用户 DTO
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }
        public string NickName { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string Portrait { get; set; }
        public string Gender { get; set; }
        public string Region { get; set; }
        public bool IsLocked { get; set; }
        public bool IsFreeze { get; set; }

        public UserRole Role { get; set; }
    }
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
