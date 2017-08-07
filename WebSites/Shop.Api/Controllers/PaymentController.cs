using ECommon.IO;
using ENode.Commanding;
using Payments.ReadModel;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Payment;
using Shop.Api.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common.Extensions;


namespace Shop.Api.Controllers
{
    /// <summary>
    /// 第三方支付
    /// </summary>
    [ApiAuthorizeFilter]
    [EnableCors(origins: "http://localhost:51776,http://localhost:8080", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class PaymentController: BaseApiController
    {
        private ICommandService _commandService;//C端
        private PaymentQueryService _paymentQueryService;//Q 端

        public PaymentController(ICommandService commandService, PaymentQueryService paymentQueryService)
        {
            _commandService = commandService;
            _paymentQueryService = paymentQueryService;
        }

        /// <summary>
        /// 微信支付
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Payment/WeChatPay")]
        public BaseApiResponse WeChatPay(PaymentRequest request)
        {
            request.CheckNotNull(nameof(request));

            return new BaseApiResponse();
        }

        /// <summary>
        /// 支付宝 支付
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Payment/WeChatPay")]
        public BaseApiResponse AliPay(PaymentRequest request)
        {
            request.CheckNotNull(nameof(request));

            return new BaseApiResponse();
        }


        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.CommandExecuted).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}