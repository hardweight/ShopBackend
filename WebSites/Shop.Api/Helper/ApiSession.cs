using Autofac;
using CacheManager.Core;
using ECommon.Autofac;
using ECommon.Components;
using Shop.Api.Extensions;
using Shop.Api.ViewModels.Carts;
using Shop.Api.ViewModels.Store;
using Shop.Api.ViewModels.User;
using Shop.Api.ViewModels.Wallet;
using Shop.ReadModel.Carts;
using Shop.ReadModel.Stores;
using Shop.ReadModel.Users;
using Shop.ReadModel.Wallets;
using System;
using System.Web;
using Xia.Common.Extensions;
using Xia.Common.Secutiry;

namespace Shop.Api.Helper
{
    /// <summary>
    /// 管理客户信息
    /// </summary>
    public class ApiSession
    {
        private volatile static ApiSession _instance = null;
        private static readonly object _lockHelper = new object();
        private ICacheManager<object> _cache;

        private UserQueryService _userQueryService;
        private WalletQueryService _walletQueryService;
        private StoreQueryService _storeQueryService;
        private CartQueryService _cartQueryService;

        private ApiSession()
        {
            //需要配置信息

            //从IOC容器中获取queryservice实例
            var container = (ObjectContainer.Current as AutofacObjectContainer).Container;
            _userQueryService= container.Resolve<UserQueryService>();
            _walletQueryService = container.Resolve<WalletQueryService>();
            _storeQueryService = container.Resolve<StoreQueryService>();
            _cartQueryService = container.Resolve<CartQueryService>();


            //缓存信息
            if (_cache == null)
            {
                _cache = CacheFactory.Build("RunTimeCache", settings =>
                {
                    settings.WithSystemRuntimeCacheHandle("handleName");
                });
            }
        }

        /// <summary>
        /// 单例模式
        /// </summary>
        /// <returns></returns>
        public static ApiSession CreateInstance()
        {
            if (_instance == null)
            {
                lock (_lockHelper)
                {
                    if (_instance == null)
                        _instance = new ApiSession();
                }
            }
            return _instance;
        }

        #region  用户信息
        /// <summary>
        /// 获取用户信息  1缓存》2数据库
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserViewModel GetUserInfo(string userId)
        {
            userId.CheckNotNullOrEmpty(nameof(userId));
            var userInfo = _cache.Get(CacheKeySupplier.UserModelCacheKey(userId)) as UserViewModel;
            if(userInfo==null)
            {
                userInfo = _userQueryService.FindUser(userId.ToGuid()).ToUserModel();
                _cache.Add(CacheKeySupplier.UserModelCacheKey(userId), userInfo);
            }
            return userInfo;
        }

        /// <summary>
        /// 设置用户模型到缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userInfo"></param>
        public void SetUserInfo(string userId,UserViewModel userInfo)
        {
            userId.CheckNotNullOrEmpty(nameof(userId));
            userInfo.CheckNotNull(nameof(userInfo));

            _cache.Add(CacheKeySupplier.UserModelCacheKey(userId), userInfo);
        }

        /// <summary>
        /// 设置短信验证码到缓存
        /// </summary>
        /// <param name="token">key</param>
        /// <param name="code"></param>
        public void SetMsgCode(string token,string code)
        {
            var cacheCode = _cache.Get(token) as string;
            if (!cacheCode.IsNullOrEmpty())
            {
                _cache.Update(token, u => code);
            }
            else
            {
                _cache.Add(token, code);
            }
        }
        /// <summary>
        /// 获取短信验证码缓存
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetMsgCode(string token)
        {
            return _cache.Get(token) as string;
        }


        /// <summary>
        /// 更新缓存的用户模型
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userInfo"></param>
        public void UpdateUserInfo(string userId, UserViewModel userInfo)
        {
            _cache.Update(CacheKeySupplier.UserModelCacheKey(userId), u => userInfo);
        }
        #endregion

        #region 钱包相关
        /// <summary>
        /// 获取信息  1缓存》2数据库
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public WalletViewModel GetWalletInfo(string walletId)
        {
            walletId.CheckNotNullOrEmpty(nameof(walletId));
            var walletInfo = _cache.Get(CacheKeySupplier.WalletModelCacheKey(walletId)) as WalletViewModel;
            if (walletInfo == null)
            {
                walletInfo = _walletQueryService.Info(walletId.ToGuid()).ToWalletModel();
                _cache.Add(CacheKeySupplier.WalletModelCacheKey(walletId), walletInfo);
            }
            return walletInfo;
        }

        /// <summary>
        /// 设置模型到缓存
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="walletInfo"></param>
        public void SetWalletInfo(string walletId, WalletViewModel walletInfo)
        {
            walletId.CheckNotNullOrEmpty(nameof(walletId));
            walletInfo.CheckNotNull(nameof(walletInfo));

            _cache.Add(CacheKeySupplier.WalletModelCacheKey(walletId), walletInfo);
        }

        public CartViewModel GetCartInfo(string cartId)
        {
            cartId.CheckNotNullOrEmpty(nameof(cartId));
            var cartInfo = _cache.Get(CacheKeySupplier.CartModelCacheKey(cartId)) as CartViewModel;
            if (cartInfo == null)
            {
                cartInfo = _cartQueryService.Info(cartId.ToGuid()).ToCartModel();
                _cache.Add(CacheKeySupplier.CartModelCacheKey(cartId), cartInfo);
            }
            return cartInfo;
        }
        public void SetCartInfo(string cartId, CartViewModel cartInfo)
        {
            cartId.CheckNotNullOrEmpty(nameof(cartId));
            cartInfo.CheckNotNull(nameof(cartInfo));

            _cache.Add(CacheKeySupplier.CartModelCacheKey(cartId), cartInfo);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="walletId"></param>
        /// <param name="walletInfo"></param>
        public void UpdateWalletInfo(string userId, WalletViewModel walletInfo)
        {
            _cache.Update(CacheKeySupplier.WalletModelCacheKey(userId), u => walletInfo);
        }
        #endregion

        #region 商家相关
        /// <summary>
        /// 获取信息  1缓存》2数据库
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public StoreViewModel GetStoreInfo(string userId)
        {
            userId.CheckNotNullOrEmpty(nameof(userId));
            var storeInfo = _cache.Get(CacheKeySupplier.StoreModelCacheKey(userId)) as StoreViewModel;
            if (storeInfo == null)
            {
                storeInfo = _storeQueryService.InfoByUserId(userId.ToGuid()).ToStoreModel();
                _cache.Add(CacheKeySupplier.StoreModelCacheKey(userId), storeInfo);
            }
            return storeInfo;
        }

        /// <summary>
        /// 设置模型到缓存
        /// </summary>
        /// <param name="useridId"></param>
        /// <param name="storeInfo"></param>
        public void SetStoreInfo(string useridId, StoreViewModel storeInfo)
        {
            useridId.CheckNotNullOrEmpty(nameof(useridId));
            storeInfo.CheckNotNull(nameof(storeInfo));

            _cache.Add(CacheKeySupplier.StoreModelCacheKey(useridId), storeInfo);
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="storeInfo"></param>
        public void UpdateStoreInfo(string userId, StoreViewModel storeInfo)
        {
            _cache.Update(CacheKeySupplier.StoreModelCacheKey(userId), u => storeInfo);
        }
        #endregion

        #region Cookie相关
        /// <summary>
        /// 获取当前登陆用户ID
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetCurrentUserId(HttpRequest request)
        {
            var cookie = request.Cookies[AuthCookieConfig.AUTH_COOKIE_NAME];
            if (cookie == null)
            {
                throw new Exception("auth cookie is null");
            }
            //解密
            return DesHelper.Encrypt(cookie.Value, AuthCookieConfig.AUTH_COOKIE_KEY);
        }

        /// <summary>
        /// 设置auth cookie
        /// </summary>
        /// <param name="response"></param>
        /// <param name="userId"></param>
        public void SetAuthCookie(HttpResponse response,string userId)
        {
            var cookie = new HttpCookie(AuthCookieConfig.AUTH_COOKIE_NAME);
            //cookie.Value = DesHelper.Encrypt(userId, AuthCookieConfig.AUTH_COOKIE_KEY) ;//加密用户数据
            cookie.Value = userId;
            cookie.Domain = AuthCookieConfig.AUTH_COOKIE_DOMAIN;
            cookie.Expires = DateTime.MaxValue;
            response.AppendCookie(cookie);
        }

        /// <summary>
        /// 删除cookie
        /// </summary>
        /// <param name="response"></param>
        public void ClearAuthCookie(HttpResponse response)
        {
            response.Cookies.Clear();
        }
        #endregion

    }
}