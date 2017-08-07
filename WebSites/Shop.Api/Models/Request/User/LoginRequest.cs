namespace Shop.Api.Models.Request.User
{
    public class LoginRequest
    {
        /// <summary>
        /// 国际电话区号
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}