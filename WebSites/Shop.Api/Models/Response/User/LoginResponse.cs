using System;

namespace Shop.Api.Models.Response.User
{
    public class LoginResponse:BaseApiResponse
    {
        public UserInfo UserInfo { get; set; }
        public WalletInfo WalletInfo { get; set; }
    }

    public class UserInfo
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string NickName { get; set; }
        public string Portrait { get; set; }
        public string Gender { get; set; }
        public string Region { get; set; }
        public string Mobile { get; set; }

        public string Role { get; set; }
        public string CartId { get; set; }
        public int CartGoodsCount { get; set; }
        public string StoreId { get; set; }
        /// <summary>
        /// 登录令牌 Token
        /// </summary>
        public string Token { get; set; }

    }

    public class WalletInfo
    {
        public Guid Id { get; set; }
        public string AccessCode { get; set; }
        public decimal Cash { get; set; }
        public decimal Benevolence { get; set; }
        public decimal YesterdayEarnings { get; set; }
        public decimal Earnings { get; set; }
    }
}