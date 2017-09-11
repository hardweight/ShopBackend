using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Admins;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Admins;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class AdminController:BaseApiController
    {
        private ICommandService _commandService;//C端

        public AdminController(ICommandService commandService)
        {
            _commandService = commandService;
        }
        
        /// <summary>
        /// 获取类别树信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("Admin/Login")]
        public BaseApiResponse Login(LoginRequest request)
        {
            request.CheckNotNull(nameof(request));

            if(request.Name!="admin" || request.Password!="wftx123456~")
            {
                return new BaseApiResponse { Code = 400, Message = "登录不被允许" };
            }
            return new LoginResponse
            {
                User=new User
                {
                    Id=GuidUtil.NewSequentialId(),
                    LoginName="admin",
                    Name="夏某某",
                    Password="123456",
                    Portrait= "https://raw.githubusercontent.com/taylorchen709/markdown-images/master/vueadmin/user.png"
                }
            };
        }

        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.EventHandled).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}