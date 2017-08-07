using System;

namespace Shop.Domain.Models.Users
{
    [Serializable]
    public class UserInfo
    {
        public string NickName { get;  set; }
        public string Portrait { get;  set; }
        public string Gender { get;  set; }
        public string Mobile { get;  set; }
        public string Region { get;  set; }
        public string Password { get;  set; }

        public UserInfo() { }
        public UserInfo(string mobile,string nickName,string portrait,string gender,string region,string password)
        {
            Mobile = mobile;
            NickName = nickName;
            Portrait = portrait;
            Gender = gender;
            Region = region;
            Password = password;
        }
    }
}
