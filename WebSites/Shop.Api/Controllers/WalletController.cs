using ECommon.IO;
using ENode.Commanding;
using Shop.Api.Extensions;
using Shop.Api.Helper;
using Shop.Api.Models.Request.Wallet;
using Shop.Api.Models.Response;
using Shop.Api.Models.Response.Wallet;
using Shop.Api.ViewModels.Wallet;
using Shop.Commands.Wallets;
using Shop.Commands.Wallets.BankCards;
using Shop.Commands.Wallets.CashTransfers;
using Shop.ReadModel.Wallets;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    /// <summary>
    /// 钱包接口
    /// </summary>
    [ApiAuthorizeFilter]
    [EnableCors(origins: "http://localhost:51776,http://localhost:8080", headers: "*", methods: "*",SupportsCredentials =true)]//接口跨越访问配置
    public class WalletController : BaseApiController
    {
        private ICommandService _commandService;//C端

        private WalletQueryService _walletQueryService;//钱包Q端
       
        
        /// <summary>
        /// IOC 构造函数注入
        /// </summary>
        /// <param name="commandService"></param>
        /// <param name="conferenceQueryService"></param>
        public WalletController(ICommandService commandService, WalletQueryService walletQueryService)
        {
            _commandService = commandService;
            _walletQueryService = walletQueryService;
        }

        #region 基本信息


        /// <summary>
        /// 获取钱包信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/Info")]
        public WalletInfoResponse Info()
        {
            TryInitUserModel();

            var walletinfo = _walletQueryService.InfoByUserId(_user.Id);
            _apiSession.SetWalletInfo(_user.Id.ToString(), walletinfo.ToWalletModel());
            //var walletinfo = _apiSession.GetWalletInfo(_user.Id.ToString());

            return new WalletInfoResponse
            {
                WalletInfo =new WalletInfo
                {
                    Id=walletinfo.Id,
                    AccessCode=walletinfo.AccessCode,
                    Cash=walletinfo.Cash,
                    Benevolence=walletinfo.Benevolence,
                    Earnings=walletinfo.Earnings,
                    YesterdayEarnings=walletinfo.YesterdayEarnings
                }
            };
        }

        /// <summary>
        /// 重置 支付密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/SetAccessCode")]
        public async Task<BaseApiResponse> SetAccessCode(SetAccessCodeRequest request)
        {
            request.CheckNotNull(nameof(request));
            if(!request.AccessCode.IsPayPassword())
            {
                return new BaseApiResponse { Code = 400, Message = "支付密码为6为数字" };
            }

            TryInitUserModel();

            var command = new SetAccessCodeCommand(_wallet.Id, request.AccessCode);

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 现金记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/CashTransfers")]
        public GetCashTransfersResponse CashTransfers()
        {
            TryInitUserModel();

            //通过以上方法 已经获取_wallet实例了
            var cashTransfers = _walletQueryService.GetCashTransfers(_wallet.Id);

            return new GetCashTransfersResponse
            {
                CashTransfers = cashTransfers.Select(x => new CashTransfer
                {
                    Number = x.Number,
                    Amount=x.Amount,
                    Remark=x.Remark,
                    CreatedOn = x.CreatedOn.ToShortDateString(),
                    Type = x.Type.ToDescription(),
                    Direction = x.Direction.ToDescription()
                }).ToList()
            };
        }

        /// <summary>
        /// 善心记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/BenevolenceTransfers")]
        public GetBenevolenceTransfersResponse BenevolenceTransfers()
        {
            TryInitUserModel();

            //通过以上方法 已经获取_wallet实例了
            var benevolenceTransfers = _walletQueryService.GetBenevolenceTransfers(_wallet.Id);

            return new GetBenevolenceTransfersResponse
            {
                BenevolenceTransfers = benevolenceTransfers.Select(x => new BenevolenceTransfer
                {
                    Number = x.Number,
                    Amount=x.Amount,
                    Remark=x.Remark,
                    CreatedOn = x.CreatedOn.ToShortDateString(),
                    Type = x.Type.ToDescription(),
                    Direction = x.Direction.ToDescription()
                }).ToList()
            };
        }

        #endregion

        #region 支付

        /// <summary>
        /// 余额支付 这里的支付金额基本上《=钱包账号余额 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/CashPay")]
        public async Task<BaseApiResponse> CashPay(CashPayRequest request)
        {
            request.CheckNotNull(nameof(request));
            TryInitUserModel();

            var walletinfo = _walletQueryService.InfoByUserId(_user.Id);
            //验证支付密码
            if (!walletinfo.AccessCode.Equals(request.AccessCode))
            {
                return new BaseApiResponse { Code = 400, Message = "支付密码错误" };
            }

            string number = DateTime.Now.ToSerialNuber();
            var command = new CreateCashTransferCommand(
                Guid.NewGuid(),
                walletinfo.Id,
                number,//流水号
                CashTransferType.Shopping,
                CashTransferStatus.Placed,//这里只是提交，只有钱包接受改记录后，才更新为成功
                request.Amount,
                0,
                WalletDirection.Out,
                "订单号"+request.OrderNumber);

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/Recharge")]
        public async Task<BaseApiResponse> Recharge(RechargeRequest request)
        {
            request.CheckNotNull(nameof(request));

            TryInitUserModel();

            var command = new CreateCashTransferCommand(
                Guid.NewGuid(),
                _wallet.Id,
                DateTime.Now.ToSerialNuber(),
                CashTransferType.Charge,
                CashTransferStatus.Placed,
                request.Amount,
                0,
                WalletDirection.In,
                "充值");

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }

            return new BaseApiResponse();
        }
        #endregion

        #region 银行卡


        /// <summary>
        /// 获取银行卡列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/BankCards")]
        public GetBankCardsResponse BankCards()
        {
            TryInitUserModel();

            //通过以上方法 已经获取_wallet实例了
            var bankCards = _walletQueryService.GetBankCards(_wallet.Id);

            return new GetBankCardsResponse
            {
                BankCards = bankCards.Select(x => new BankCard
                {
                    Id = x.Id,
                    WalletId=x.WalletId,
                    BankName = x.BankName,
                    OwnerName=x.OwnerName,
                    Number = x.Number
                }).ToList()
            };
        }

        /// <summary>
        /// 添加银行卡
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/AddBankCard")]
        public async Task<BaseApiResponse> AddBankCard(AddBankCardRequest request)
        {
            request.CheckNotNull(nameof(request));

            TryInitUserModel();

            var bankCardViewModel = new BankCardViewModel
            {
                WalletId = _wallet.Id,//怎样获取钱包ID
                BankName = request.BankName,
                OwnerName = request.OwnerName,
                Number = request.Number
            };
            var command = bankCardViewModel.ToAddBankCardCommand();

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="expressAddressId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/DeleteBankCard")]
        public async Task<BaseApiResponse> DeleteBankCard(DeleteBankCardRequest request)
        {
            request.CheckNotNull(nameof(request));

            TryInitUserModel();

            var command = new RemoveBankCardCommand(_wallet.Id, request.BankCardId);
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
