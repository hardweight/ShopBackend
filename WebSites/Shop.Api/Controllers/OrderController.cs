using Buy.ReadModel;
using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Cors;
using Shop.Api.Extensions;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    [ApiAuthorizeFilter]
    [EnableCors(origins: "http://localhost:51776,http://localhost:8080", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class OrderController:BaseApiController
    {
        private ICommandService _commandService;//C端
        private OrderQueryService _orderQueryService;//Q 端

        public OrderController(ICommandService commandService, OrderQueryService orderQueryService)
        {
            _commandService = commandService;
            _orderQueryService = orderQueryService;
        }



        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.CommandExecuted).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}