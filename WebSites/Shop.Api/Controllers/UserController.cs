using Buy.ReadModel;
using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.User;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.User;
using Shop.Api.ViewModels.User;
using Shop.Commands.Users;
using Shop.Commands.Users.ExpressAddresses;
using Shop.Commands.Users.UserGifts;
using Shop.ReadModel.Stores;
using Shop.ReadModel.Users;
using Shop.ReadModel.Wallets;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common;
using Xia.Common.Extensions;
using Xia.Common.Secutiry;
using ApiResponse = Shop.Api.Models.Response.User;

namespace Shop.Api.Controllers
{
    /// <summary>
    /// 用户接口
    /// </summary>
    [ApiAuthorizeFilter]
    [EnableCors(origins: "http://localhost:51776,http://localhost:8080", headers: "*", methods: "*",SupportsCredentials =true)]//接口跨越访问配置
    public class UserController : BaseApiController
    {
        private ICommandService _commandService;//C端

        private UserQueryService _userQueryService;//用户Q端
        private WalletQueryService _walletQueryService;//钱包Q端
        private OrderQueryService _orderQueryService;//订单Q端
        private StoreQueryService _storeQueryService;//商家
       
        
        /// <summary>
        /// IOC 构造函数注入
        /// </summary>
        /// <param name="commandService"></param>
        /// <param name="conferenceQueryService"></param>
        public UserController(ICommandService commandService, 
            UserQueryService userQueryService,
            WalletQueryService walletQueryService, 
            OrderQueryService orderQueryService,
            StoreQueryService storeQueryService)
        {
            _commandService = commandService;
            _userQueryService = userQueryService;
            _walletQueryService = walletQueryService;
            _orderQueryService = orderQueryService;
            _storeQueryService = storeQueryService;
        }
        

        #region 登陆 注册

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/SendMsgCode")]
        [AllowAnonymous]
        public BaseApiResponse SendMsgCode(SendMsgCodeRequest request)
        {
            request.CheckNotNull(nameof(request));
            if(!request.Mobile.IsMobileNumber())
            {
                return new BaseApiResponse { Code = 400, Message = "错误的手机号码" };
            }
            //创建验证码
            var code = new Random().GetRandomNumberString(6);
            var token = Guid.NewGuid().ToString();

            //发送验证码短信

            //验证码缓存 设置过期期限缓存策略默认已设置好
            _apiSession.SetMsgCode(token, code);

            return new ApiResponse.SendMsgCodeResponse() {
                Token=token,
                MsgCode=code
            };
        }
        /// <summary>
        /// 检查手机号是否可以注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/CheckPhoneAvailable")]
        [AllowAnonymous]
        public BaseApiResponse CheckPhoneAvailable(CheckPhoneAvailableRequest request)
        {
            request.CheckNotNull(nameof(request));

            if(!request.Phone.IsMobileNumber())
            {
                return new BaseApiResponse { Code=400, Message="错误的手机号码" };
            }

            if (_userQueryService.CheckMobileIsAvliable(request.Phone))
            {
                return new CheckPhoneAvailableResponse { Result = true};
            }
            else
            {
                return new CheckPhoneAvailableResponse { Result=false, Message= "该手机号已经注册."};
            }
        }

        [HttpPost]
        [Route("User/VerifyMsgCode")]
        [AllowAnonymous]
        public BaseApiResponse VerifyMsgCode(VerifyMsgCodeRequest request)
        {
            request.CheckNotNull(nameof(request));
            //验证码验证
            if (request.Token.IsNullOrEmpty() || _apiSession.GetMsgCode(request.Token) == null)
            {
                return new BaseApiResponse { Code = 400, Message = "验证码过期" };
            }
            if (_apiSession.GetMsgCode(request.Token) != request.MsgCode)
            {
                return new BaseApiResponse { Code = 400, Message = "验证码错误" };
            }
            return new BaseApiResponse();
        }
        /// <summary>
        /// 注册新用户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/Register")]
        [AllowAnonymous]
        public async Task<BaseApiResponse> Register(RegisterRequest request)
        {
            request.CheckNotNull(nameof(request));
            //验证码验证
            if (request.Token.IsNullOrEmpty() || _apiSession.GetMsgCode(request.Token)==null)
            {
                return new BaseApiResponse { Code = 400, Message = "验证码过期" };
            }
            if(_apiSession.GetMsgCode(request.Token)!=request.MsgCode)
            {
                return new BaseApiResponse { Code = 400, Message = "验证码错误" };
            }

            if (!request.Mobile.IsMobileNumber())
            {
                return new BaseApiResponse { Code = 400, Message = "手机号不正确" };
            }
            if (request.Password.Length > 20)
            {
                return new BaseApiResponse { Code = 400, Message = "密码长度不能大于20字符" };
            }
            if (request.Password.Contains(" "))
            {
                return new BaseApiResponse { Code = 400, Message = "密码不能包含空格." };
            }

            //检查手机号是否可用
            if(!_userQueryService.CheckMobileIsAvliable(request.Mobile))
            {
                return new BaseApiResponse { Code = 400, Message = "该手机号已注册." };
            }

            //创建用户command
            var userViewModel = new UserViewModel {
                Id= GuidUtil.NewSequentialId(),
                Mobile = request.Mobile,
                NickName ="用户"+ StringGenerator.Generate(4),//创建随机昵称
                Password=request.Password,
                Portrait="default.jpg",
                Region="北京",
                Gender="男"
            };
            var command = userViewModel.ToCreateUserCommand();
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message ="命令没有执行成功：{0}".FormatWith( result.GetErrorMessage() )};
            }

            return new RegisterResponse
            {
                Result = new RegisterResult
                {
                    Id = command.AggregateRootId.ToString()
                }
            };
        }

        /// <summary>
        /// 用户登录。返回用户基本信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/Login")]
        [AllowAnonymous]
        public BaseApiResponse Login(LoginRequest request)
        {
            request.CheckNotNull(nameof(request));
            if(!request.Mobile.IsMobileNumber())
            {//是否手机号
                return new BaseApiResponse { Code = 400 ,Message="手机号格式不正确"};
            }
            var userinfo = _userQueryService.FindUser(request.Mobile);
            //验证用户
            if(userinfo==null)
            {
                return new BaseApiResponse{Code = 1000, Message = "没找到该账号"};
            }
            //验证密码
            if(!PasswordHash.ValidatePassword(request.Password,userinfo.Password))
            {
                return new BaseApiResponse{ Code = 1000,  Message = "登录密码错误"};
            }
            //设置cookie 和缓存
            _apiSession.SetAuthCookie(HttpContext.Current.Response, userinfo.Id.ToString());
            _apiSession.SetUserInfo(userinfo.Id.ToString(),userinfo.ToUserModel());

            //获取钱包信息
            var walletinfo = _walletQueryService.InfoByUserId(userinfo.Id);
            if(walletinfo==null)
            {
                return new BaseApiResponse { Code = 1000, Message = "获取钱包信息失败" };
            }
            //店铺信息
            var storeId = "";
            var storeinfo = _storeQueryService.InfoByUserId(userinfo.Id);
            if(storeinfo!=null)
            {
                storeId = storeinfo.Id.ToString();
            }

            return new LoginResponse
            {
                UserInfo = new UserInfo
                {
                    Id = userinfo.Id,
                    NickName = userinfo.NickName,
                    Portrait = userinfo.Portrait,
                    Mobile = userinfo.Mobile,
                    Gender = userinfo.Gender,
                    Region = userinfo.Region,
                    Role = userinfo.Role.ToDescription(),
                    StoreId = storeId,
                    Token = userinfo.Id.ToString()
                },
                WalletInfo = new ApiResponse.WalletInfo
                {
                    Id = walletinfo.Id,
                    AccessCode=walletinfo.AccessCode,
                    Cash=walletinfo.Cash,
                    Benevolence=walletinfo.Benevolence,
                    Earnings=walletinfo.Earnings,
                    YesterdayEarnings=walletinfo.YesterdayEarnings
                }
            };
        }

        /// <summary>
        /// 当前用户注销。需要登录才能访问。
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("User/Logout")]
        public BaseApiResponse Logout()
        {
            _apiSession.ClearAuthCookie(HttpContext.Current.Response);
            return new BaseApiResponse();
        }
        #endregion

        #region 收货地址

        /// <summary>
        /// 获取当前用户的所以收货地址
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("User/ExpressAddresses")]
        public ApiResponse.ExpressAddressesResponse ExpressAddresses()
        {
            TryInitUserModel();
            //用户登录成功就可用通过token 初始化_user 对象 ，代表当前登录用户

            //应该从缓存中获取用户的地址信息
            var addresses = _userQueryService.GetExpressAddresses(_user.Id);
            return new ExpressAddressesResponse
            {
                ExpressAddresses = addresses.Select(x => new ApiResponse.ExpressAddress
                {
                    Id = x.Id,
                    Name = x.Name,
                    Mobile = x.Mobile,
                    Address = x.Address,
                    Region = x.Region,
                    Zip = x.Zip
                }).ToList()
            };
        }


        /// <summary>
        /// 添加收货地址 只有登录才可访问
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/AddExpressAddress")]
        public async Task<BaseApiResponse> AddExpressAddress(AddExpressAddressRequest request)
        {
            TryInitUserModel();
            request.CheckNotNull(nameof(request));
            if (!request.Mobile.IsMobileNumber())
            {
                return new BaseApiResponse { Code = 400, Message = "请输入正确的手机号" };
            }
            if (!request.Zip.IsZipCode())
            {
                return new BaseApiResponse { Code = 400, Message = "请输入正确的邮编" };
            }

            var expressAddressViewModel = new ExpressAddressViewModel
            {
                UserId = _user.Id,
                Mobile = request.Mobile,
                Name = request.Name,
                Region = request.Region,
                Address = request.Address,
                Zip = request.Zip
            };
            var command = expressAddressViewModel.ToAddExpressAddressCommand();
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="expressAddressId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/DeleteExpressAddress")]
        public async Task<BaseApiResponse> DeleteExpressAddress(DeleteExpressAddressRequest request)
        {
            request.CheckNotNull(nameof(request));

            TryInitUserModel();

            var command = new RemoveExpressAddressCommand(_user.Id, request.ExpressAddressId);
            var result = await ExecuteCommandAsync(command);

            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }
        #endregion

        #region 基本信息


        /// <summary>
        /// 获取用户信息 包含一些统计信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("User/MeInfo")]
        public MeInfoResponse MeInfo()
        {
            TryInitUserModel();

            var userinfo = _userQueryService.FindUser(_user.Id);
            var walletinfo = _walletQueryService.InfoByUserId(_user.Id);

            _apiSession.SetUserInfo(userinfo.Id.ToString(), userinfo.ToUserModel());
            _apiSession.SetWalletInfo(_user.Id.ToString(), walletinfo.ToWalletModel());

            //店铺信息
            var storeId = "";
            var storeinfo = _storeQueryService.InfoByUserId(userinfo.Id);
            if (storeinfo != null)
            {
                storeId = storeinfo.Id.ToString();
            }

            return new MeInfoResponse
            {
                UserInfo = new UserInfo
                {
                    Id = userinfo.Id,
                    NickName= userinfo.NickName,
                    Gender= userinfo.Gender,
                    Portrait= userinfo.Portrait,
                    Region= userinfo.Region,
                    Mobile= userinfo.Mobile,
                    Role= userinfo.Role.ToDescription(),
                    StoreId = storeId,
                    Token = userinfo.Id.ToString()
                },
                WalletInfo =new WalletInfo
                {
                    Id= walletinfo.Id,
                    AccessCode= walletinfo.AccessCode,
                    Cash= walletinfo.Cash,
                    Benevolence= walletinfo.Benevolence,
                    Earnings= walletinfo.Earnings,
                    YesterdayEarnings= walletinfo.YesterdayEarnings
                }
            };
        }

        

       

        /// <summary>
        /// 通过手机验证码设置新密码 需要登录才能访问
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/ResetPassword")]
        public BaseApiResponse ResetPassword(ResetPasswordRequest request)
        {
            TryInitUserModel();

            return new BaseApiResponse();
        }

        /// <summary>
        /// 当前登录用户通过旧密码设置新密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/ChangePassword")]
        public BaseApiResponse ChangePassword(ChangePasswordRequest request)
        {
            //验证原密码的正确性

            TryInitUserModel();


            return new BaseApiResponse();
        }

        /// <summary>
        /// 设置当前用户的昵称 需要登录才能访问
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/SetNickName")]
        public async Task<BaseApiResponse> SetNickName(SetNickNameRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();

            if(request.NickName.Length>20)
            {
                return new BaseApiResponse { Code = 400, Message = "昵称长度不得超过20字符." };
            }
            //更新
            var command = new UpdateNickNameCommand(request.NickName) { AggregateRootId = _user.Id };
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "昵称设置失败" };
            }
           
            //更新缓存
            _user.NickName = request.NickName;
            _apiSession.UpdateUserInfo(_user.Id.ToString(), _user);

            return new BaseApiResponse();
        }

        /// <summary>
        /// 设置当前用户头像地址 需要登录才能访问
        /// </summary>
        /// <param name="portraitUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/SetPortrait")]
        public async Task<BaseApiResponse> SetPortrait(SetPortraitRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();
            
            //更新
            var command = new UpdatePortraitCommand(request.Portrait) { AggregateRootId = _user.Id  };
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "头像设置失败" };
            }
           
            //更新缓存
            _user.Portrait = request.Portrait;
            _apiSession.UpdateUserInfo(_user.Id.ToString(), _user);

            return new BaseApiResponse();
        }


        /// <summary>
        /// 设置当前用户的性别 需要登陆才能访问
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/SetGender")]
        public async Task<BaseApiResponse> SetGender(SetGenderRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();

            if (!"男,女".IsIncludeItem(request.Gender))
            {
                return new BaseApiResponse { Code = 400, Message = "性别参数错误，非： 男/女" };
            }
            //更新
            var command = new UpdateGenderCommand(request.Gender) { AggregateRootId = _user.Id };
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "性别设置失败" };
            }
            
            //更新缓存
            _user.Gender = request.Gender;
            _apiSession.UpdateUserInfo(_user.Id.ToString(), _user);

            return new BaseApiResponse();
        }
        /// <summary>
        /// 设置当前用户的地区 需要登陆才能访问
        /// </summary>
        /// <param name="gender"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/SetRegion")]
        public async Task<BaseApiResponse> SetRegion(SetRegionRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();
           
            //更新
            var command = new UpdateRegionCommand(request.Region) { AggregateRootId = _user.Id };
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "地区设置失败" };
            }
            
            //更新缓存
            _user.Region = request.Region;
            _apiSession.UpdateUserInfo(_user.Id.ToString(), _user);

            return new BaseApiResponse();
        }


        #endregion

        #region 开通大使流程

        
        /// <summary>
        /// 添加礼物和收货信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/AdduserGift")]
        public async Task<BaseApiResponse> AddUserGift(AddUserGiftRequest request)
        {
            request.CheckNotNull(nameof(request));
            request.GiftInfo.CheckNotNull(nameof(request.GiftInfo));
            request.ExpressAddressInfo.CheckNotNull(nameof(request.ExpressAddressInfo));

            TryInitUserModel();

            //要将新的ID 返回给前端
            var userGiftId = Guid.NewGuid();
            var command = new AddUserGiftCommand(
                userGiftId,
                _user.Id,
                request.GiftInfo.Name,
                request.GiftInfo.Size,
                request.ExpressAddressInfo.Name,
                request.ExpressAddressInfo.Mobile,
                request.ExpressAddressInfo.Region,
                request.ExpressAddressInfo.Address,
                request.ExpressAddressInfo.Zip);

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new AddUserGiftResponse {
                UserGiftId=userGiftId
            };

        }

        /// <summary>
        /// 获取用户未支付礼物
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("User/GetUserUnPayGift")]
        public BaseApiResponse GetUserUnPayGift()
        {
            TryInitUserModel();

            var unPayUserGift = _userQueryService.UserGifts(_user.Id).Where(x => x.Remark == "未支付").SingleOrDefault();

            if (unPayUserGift != null)
            {
                return new GetUserUnPayGiftResponse
                {
                    UserGift = new UserGift
                    {
                        Id=unPayUserGift.Id,
                        GiftName = unPayUserGift.GiftName,
                        GiftSize = unPayUserGift.GiftSize,
                        Name = unPayUserGift.Name,
                        Mobile = unPayUserGift.Mobile,
                        Region = unPayUserGift.Region,
                        Address = unPayUserGift.Address,
                        Zip = unPayUserGift.Zip
                    }
                };
            }
            return new BaseApiResponse { Code=400,Message="没有未支付"};
        }

        /// <summary>
        /// 设置礼物已付款
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("User/SetUserGiftPayed")]
        public async Task<BaseApiResponse> SetUserGiftPayed(SetUserGiftPayedRequest request)
        {
            request.CheckNotNull(nameof(request));
            request.UserGiftId.CheckNotEmpty(nameof(request.UserGiftId));
            TryInitUserModel();
            //设置支付成功
            var command = new SetUserGiftPayedCommand(_user.Id, request.UserGiftId);
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }



        #endregion
        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.CommandExecuted).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}
