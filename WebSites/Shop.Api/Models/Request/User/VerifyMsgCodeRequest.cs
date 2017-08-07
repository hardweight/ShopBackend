namespace Shop.Api.Models.Request.User
{
    public class VerifyMsgCodeRequest
    {
        public string Token { get; set; }
        public string MsgCode { get; set; }
    }
}