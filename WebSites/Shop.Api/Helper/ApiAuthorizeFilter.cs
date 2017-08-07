using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Xia.Common.Secutiry;
using Xia.Common.Extensions;

namespace Shop.Api.Helper
{
    /// <summary>
    /// 接口身份验证
    /// </summary>
    public class ApiAuthorizeFilter: ActionFilterAttribute, IDisposable
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            bool isLogin = false;
            //用户登录验证 从客户段获取cookie并验证

            var cookie = actionContext.Request.Headers.GetCookies();
            if (cookie != null && cookie.Count > 0)
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
                    isLogin = true;
                }
            }
            //isLogin = true or false;

            if (isLogin)
            {
                //如果已经登录，则跳过验证
                base.OnActionExecuting(actionContext);
            }
            else
            {
                //如果请求Header不包含登陆cookie，则判断是否是匿名调用
                var attr = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>();
                bool isAnonymous = attr.Any(a => a is AllowAnonymousAttribute);

                //是匿名用户，则继续执行；非匿名用户，抛出“未授权访问”信息 ( 抛出401 )
                if (isAnonymous)
                    base.OnActionExecuting(actionContext);
                else
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

        }


        #region IDisposable
        /// <summary>
        /// 实现接口IDisposable
        /// </summary>
        public void Dispose()
        {
            //if (DbContext != null)
            //{
            //    DbContext.Dispose();
            //    DbContext = null;
            //}
        }
        #endregion
    }
}