using Shop.Api.Helper;
using Shop.Api.ViewModels.User;
using Shop.Api.ViewModels.Wallet;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    public class BaseApiController : ApiController
    {
        protected UserViewModel _user;
        protected WalletViewModel _wallet;

        protected ApiSession _apiSession=ApiSession.CreateInstance();
        
        

        /// <summary>
        /// 如果用户已经登陆 获取用户信息
        /// </summary>
        protected void TryInitUserModel()
        {
            //从cookie 获取登陆用户ID
            //创建用户信息可以先从缓存中获取，没有在从数据库取
            var cookie = this.Request.Headers.GetCookies();
            if (cookie != null || cookie.Count > 0)
            {
                string userId = string.Empty;
                foreach (var perCookie in cookie[0].Cookies)
                {
                    if (perCookie.Name.Equals(AuthCookieConfig.AUTH_COOKIE_NAME))
                    {
                        //从cookie 中解密出用户ID
                        //userId = DesHelper.Encrypt(perCookie.Value, AuthCookieConfig.AUTH_COOKIE_KEY);
                        userId = perCookie.Value;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(userId) && userId.IsGuid())
                {
                    //从缓存获取用户信息
                    _user = _apiSession.GetUserInfo(userId);
                    //从缓存获取钱包信息
                    _wallet = _apiSession.GetWalletInfo(_user.WalletId.ToString());
                }
            }
        }


        protected dynamic ThrowApiError(string message, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest)
        {
            var resp = new HttpResponseMessage(httpStatusCode) { Content = new StringContent(message) };
            throw new HttpResponseException(resp);
        }

        
    }
}
