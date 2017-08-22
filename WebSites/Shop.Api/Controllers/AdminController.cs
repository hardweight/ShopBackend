using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Helper;
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
    [ApiAuthorizeFilter]
    [EnableCors(origins: "http://app.wftx666.com,http://localhost:51776,http://localhost:8080", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
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
        public BaseApiResponse Login()
        {
            
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
            return _commandService.ExecuteAsync(command, CommandReturnType.CommandExecuted).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}