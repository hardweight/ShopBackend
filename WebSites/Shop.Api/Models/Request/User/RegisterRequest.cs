namespace Shop.Api.Models.Request.User
{
    /// <summary>
    ///  请求DTO
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
       
        /// <summary>
        /// 短信验证码
        /// </summary>
        public string MsgCode { get; set; }

        /// <summary>
        /// 短信验证码token
        /// </summary>
        public string Token { get; set; }
        
    }
}