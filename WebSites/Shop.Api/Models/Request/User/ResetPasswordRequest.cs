namespace Shop.Api.Models.Request.User
{
    public class ResetPasswordRequest
    {
        /// <summary>
        /// 新密码，6 到 20 个字节，不能包含空格
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 调用 /user/verifycode 成功后返回的 verificationtoken
        /// </summary>
        public string VerificationToken { get; set; }
    }
}