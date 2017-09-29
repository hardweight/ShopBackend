
using ECommon.IO;
using ENode.Commanding;
using Shop.Api.AliPayAPI;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Payment;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Payments;
using Shop.Api.WxPayAPI;
using Shop.Api.WxPayAPI.lib;
using Shop.Commands.Payments;
using Shop.ReadModel.Payments;
using Shop.ReadModel.Payments.Dtos;
using System;
using System.Threading;
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
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class PaymentController: BaseApiController
    {
        private const int WaitTimeoutInSeconds = 5;

        private ICommandService _commandService;//C端
        private PaymentQueryService _paymentQueryService;//Q 端

        public PaymentController(ICommandService commandService, 
            PaymentQueryService paymentQueryService)
        {
            _commandService = commandService;
            _paymentQueryService = paymentQueryService;
        }

        /// <summary>
        /// 第三方支付成功
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Payment/PaymentAccepted")]
        public async Task<BaseApiResponse> PaymentAccepted(ProcessorPaymentRequest request)
        {
            request.CheckNotNull(nameof(request));

            var paymentDTO = WaitUntilAvailable(request.PaymentId);
            if (paymentDTO == null)
            {
                return new BaseApiResponse { Code = 400, Message = "没有支付项目" };
            }

            var command =new CompletePaymentCommand { AggregateRootId = request.PaymentId };
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }
        /// <summary>
        /// 第三方支付拒绝
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Payment/PaymentRejected")]
        public async Task<BaseApiResponse> PaymentRejected(ProcessorPaymentRequest request)
        {
            request.CheckNotNull(nameof(request));
            var paymentDTO = WaitUntilAvailable(request.PaymentId);
            if (paymentDTO == null)
            {
                return new BaseApiResponse { Code = 400, Message = "没有支付项目" };
            }

            var command=new CancelPaymentCommand { AggregateRootId = request.PaymentId };
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
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

            //向微信提交订单获取prepayid
            var apiPay = new ApiPay(Convert.ToInt32(request.Amount));
            
            WxPayData data = new WxPayData();
            data.SetValue("appid", WxPayConfig.APPID);
            data.SetValue("partnerid", WxPayConfig.MCHID);
            data.SetValue("prepayid", apiPay.GeneratePrepayId());
            data.SetValue("package", "Sign=WXPay");
            data.SetValue("noncestr", WxPayApi.GenerateNonceStr());
            data.SetValue("timestamp",WxPayApi.GenerateTimeStamp());
            data.SetValue("sign", data.MakeSign());//签名

            return new WeChatPayResponse {
                partnerid=data.GetValue("partnerid").ToString(),
                prepayid=data.GetValue("prepayid").ToString(),
                noncestr=data.GetValue("noncestr").ToString(),
                timestamp=data.GetValue("timestamp").ToString(),
                sign=data.GetValue("sign").ToString()
            };
        }

        /// <summary>
        /// 支付宝 支付
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Payment/AliPay")]
        public BaseApiResponse AliPay(PaymentRequest request)
        {
            request.CheckNotNull(nameof(request));

            var orderInfo = AliPayApi.GetAlipayOrderInfo(request.Amount,DateTime.Now.ToSerialNumber());

            return new BaseApiResponse {
                Message= orderInfo
            };
        }


        #region 私有方法

        /// <summary>
        /// 直到获取付款信息
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        private Payment WaitUntilAvailable(Guid paymentId)
        {
            var deadline = DateTime.Now.AddSeconds(WaitTimeoutInSeconds);

            while (DateTime.Now < deadline)
            {
                var paymentDTO = _paymentQueryService.FindPayment(paymentId);
                if (paymentDTO != null)
                {
                    return paymentDTO;
                }

                Thread.Sleep(500);
            }

            return null;
        }

        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.EventHandled).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}