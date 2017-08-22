using Shop.Common.Enums;
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
        public Guid ParentId { get; set; }
        public Guid WalletId { get; set; }
        public Guid CartId { get; set; }
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
    

}
