namespace Shop.Api.Models.Request.User
{
    public class MsgLoginRequest
    {
        public string Mobile { get; set; }
        public string MsgCode { get; set; }
        public string Token { get; set; }
    }
}