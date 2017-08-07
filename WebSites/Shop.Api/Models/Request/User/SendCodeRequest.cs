namespace Shop.Api.Models.Request.User
{
    /// <summary>
    /// 发送验证码 请求DTO
    /// </summary>
    public class SendCodeRequest
    {
        public string Region { get; set; }
        public string Phone { get; set; }
    }
}