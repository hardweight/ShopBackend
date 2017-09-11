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
using Shop.Commands.Wallets.BenevolenceTransfers;
using Shop.Commands.Wallets.CashTransfers;
using Shop.Commands.Wallets.RechargeApplys;
using Shop.Commands.Wallets.WithdrawApplys;
using Shop.Common;
using Shop.Common.Enums;
using Shop.ReadModel.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Xia.Common;
using Xia.Common.Extensions;

namespace Shop.Api.Controllers
{
    /// <summary>
    /// 钱包接口
    /// </summary>
    [ApiAuthorizeFilter]
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]//接口跨越访问配置
    public class WalletController : BaseApiController
    {
        private ICommandService _commandService;//C端

        private WalletQueryService _walletQueryService;//钱包Q端
       
        
        /// <summary>
        /// IOC 构造函数注入
        /// </summary>
        /// <param name="commandService"></param>
        /// <param name="conferenceQueryService"></param>
        public WalletController(ICommandService commandService, 
            WalletQueryService walletQueryService)
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

            var walletinfo = _walletQueryService.Info(_user.WalletId);
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
        /// 用户的现金记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/CashTransfers")]
        public GetCashTransfersResponse CashTransfers(CashTransfersRequest request)
        {
            request.CheckNotNull(nameof(request));

            TryInitUserModel();
            int pageRecordCount = 10;

            IEnumerable<ReadModel.Wallets.Dtos.CashTransfer> cashTransfers = null;
            //通过以上方法 已经获取_wallet实例了
            if (request.Type==CashTransferType.All)
            {
                cashTransfers = _walletQueryService.GetCashTransfers(_wallet.Id).OrderByDescending(x => x.CreatedOn).Skip(pageRecordCount * request.Page).Take(pageRecordCount);
            }
            else
            {
                cashTransfers = _walletQueryService.GetCashTransfers(_wallet.Id, request.Type).OrderByDescending(x => x.CreatedOn).Skip(pageRecordCount * request.Page).Take(pageRecordCount);
            }

            return new GetCashTransfersResponse
            {
                CashTransfers = cashTransfers.Select(x => new CashTransfer
                {
                    Number = x.Number,
                    Amount=x.Amount,
                    Fee=x.Fee,
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
        public GetBenevolenceTransfersResponse BenevolenceTransfers(BenevolenceTransfersRequest request)
        {
            request.CheckNotNull(nameof(request));

            TryInitUserModel();
            int pageRecordCount = 10;
            IEnumerable<ReadModel.Wallets.Dtos.BenevolenceTransfer> benevolenceTransfers = null;
            if(request.Type==BenevolenceTransferType.All)
            {
                benevolenceTransfers = _walletQueryService.GetBenevolenceTransfers(_wallet.Id).OrderByDescending(x => x.CreatedOn).Skip(pageRecordCount * request.Page).Take(pageRecordCount);
            }
            else
            {
                benevolenceTransfers = _walletQueryService.GetBenevolenceTransfers(_wallet.Id, request.Type).OrderByDescending(x => x.CreatedOn).Skip(pageRecordCount * request.Page).Take(pageRecordCount);
            }

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

            var walletinfo = _walletQueryService.Info(_user.WalletId);
            //验证支付密码
            if (!walletinfo.AccessCode.Equals(request.AccessCode))
            {
                return new BaseApiResponse { Code = 400, Message = "支付密码错误" };
            }

            string number = DateTime.Now.ToSerialNumber();
            var command = new CreateCashTransferCommand(
                GuidUtil.NewSequentialId(),
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
                GuidUtil.NewSequentialId(),
                _wallet.Id,
                DateTime.Now.ToSerialNumber(),
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

        #region 线下提现

        /// <summary>
        /// 提现申请记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/WithdrawApplyLogs")]
        public WithdrawApplyLogsResponse WithdrawApplyLogs()
        {
            TryInitUserModel();

            //通过以上方法 已经获取_wallet实例了
            var withdrawApplylogs = _walletQueryService.WithdrawApplyLogs(_wallet.Id);

            return new WithdrawApplyLogsResponse
            {
                WithdrawApplys = withdrawApplylogs.Select(x => new WithdrawApply
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    BankName = x.BankName,
                    BankNumber = x.BankNumber,
                    BankOwner = x.BankOwner,
                    CreatedOn = x.CreatedOn,
                    Remark = x.Remark,
                    Status = x.Status.ToDescription(),
                    WalletId=x.WalletId
                }).ToList()
            };
        }
        
        /// <summary>
        /// 提现申请
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/ApplyWithdraw")]
        public async Task<BaseApiResponse> ApplyWithdraw(ApplyWithdrawRequest request)
        {
            request.CheckNotNull(nameof(request));

            TryInitUserModel();
            //判断提现限额
            var wallet = _walletQueryService.Info(_wallet.Id);
            if (wallet.TodayWithdrawAmount + request.Amount > ConfigSettings.OneDayWithdrawLimit)
            {
                return new BaseApiResponse { Code = 400, Message = "单日提现不得超过{0}元".FormatWith(ConfigSettings.OneDayWithdrawLimit) };
            }
            if (wallet.WeekWithdrawAmount + request.Amount > ConfigSettings.OneWeekWithdrawLimit)
            {
                return new BaseApiResponse { Code = 400, Message = "每周提现不得超过{0}元".FormatWith(ConfigSettings.OneWeekWithdrawLimit) };
            }

            var command = new CreateWithdrawApplyCommand(
                GuidUtil.NewSequentialId(),
                _wallet.Id,
                request.Amount,
                request.BankCard.BankName,
                request.BankCard.Number,
                request.BankCard.OwnerName);

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

       
        

        #endregion

        #region 线下充值
        /// <summary>
        /// 线下充值申请
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/ApplyRecharge")]
        public async Task<BaseApiResponse> ApplyRecharge(ApplyRechargeRequest request)
        {
            request.CheckNotNull(nameof(request));

            TryInitUserModel();
            var command = new CreateRechargeApplyCommand(
                GuidUtil.NewSequentialId(),
                _wallet.Id,
                request.Amount,
                request.Pic,
                request.BankCard.BankName,
                request.BankCard.Number,
                request.BankCard.OwnerName);

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }
        /// <summary>
        /// 获取充值记录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Wallet/RechargeApplyLogs")]
        public RechargeApplyLogsResponse RechargeApplyLogs()
        {
            TryInitUserModel();

            //通过以上方法 已经获取_wallet实例了
            var rechargeApplylogs = _walletQueryService.RechargeApplyLogs(_wallet.Id);

            return new RechargeApplyLogsResponse
            {
                RechargeApplys = rechargeApplylogs.Select(x => new RechargeApply
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Pic=x.Pic,
                    BankName = x.BankName,
                    BankNumber = x.BankNumber,
                    BankOwner = x.BankOwner,
                    CreatedOn = x.CreatedOn,
                    Remark = x.Remark,
                    Status = x.Status.ToDescription(),
                    WalletId = x.WalletId
                }).ToList()
            };
        }




        #endregion

        #region 后台管理
        /// <summary>
        /// 激励用户的善心
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("WalletAdmin/IncentiveBenevolence")]
        public async Task<BaseApiResponse> IncentiveBenevolence(IncentiveBenevolenceRequest request)
        {
            request.CheckNotNull(nameof(request));
            if(request.BenevolenceIndex<=0 || request.BenevolenceIndex>=1)
            {
                return new BaseApiResponse { Code = 400, Message = "善心指数异常" };
            }
            //遍历所有的钱包发送激励指令
            var wallets = _walletQueryService.ListPage();
            foreach (var wallet in wallets)
            {
                var command = new IncentiveBenevolenceCommand(wallet.Id, request.BenevolenceIndex);
                await ExecuteCommandAsync(command);
            }

            return new BaseApiResponse();
        }
        /// <summary>
        /// 所有善心量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("WalletAdmin/TotalBenevolence")]
        public BaseApiResponse TotalBenevolence()
        {
            var totalBenevolence = _walletQueryService.TotalBenevolence();
            return new TotalBenevolenceResponse
            {
                TotalBenevolence = totalBenevolence
            };
        }
        /// <summary>
        /// 钱包列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("WalletAdmin/ListPage")]
        public ListPageResponse ListPage(ListPageRequest request)
        {
            request.CheckNotNull(nameof(request));

            var wallets = _walletQueryService.ListPage();
            var pageSize = 20;
            var total = wallets.Count();

            if (!request.Mobile.IsNullOrEmpty())
            {
                wallets = wallets.Where(x => x.OwnerMobile.Contains(request.Mobile)).Skip(pageSize * (request.Page - 1)).Take(pageSize);
                total = wallets.Count();
            }
            else
            {
                wallets = wallets.Skip(pageSize * (request.Page - 1)).Take(pageSize);
            }

            return new ListPageResponse
            {
                Total = total,
                Wallets = wallets.Select(x => new Wallet
                {
                    Id = x.Id,
                    OwnerMobile=x.OwnerMobile,
                    Cash=x.Cash,
                    Benevolence=x.Benevolence,
                    BenevolenceTotal=x.BenevolenceTotal,
                    TodayBenevolenceAdded=x.TodayBenevolenceAdded,
                    YesterdayEarnings=x.YesterdayEarnings,
                    Earnings=x.Earnings,
                    AccessCode=x.AccessCode,
                    YesterdayIndex=x.YesterdayIndex
                }).ToList()
            };
        }

        /// <summary>
        /// 增减现金
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("WalletAdmin/AddCashTransfer")]
        public async Task<BaseApiResponse> AddCashTransfer(AddCashTransferRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new CreateCashTransferCommand(
                GuidUtil.NewSequentialId(),
                request.Id,
                DateTime.Now.ToSerialNumber(),
                CashTransferType.SystemOp,
                CashTransferStatus.Placed,
                request.Amount,
                0,
                request.Direction,
                request.Remark);
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 增减善心量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("WalletAdmin/AddBenevolenceTransfer")]
        public async Task<BaseApiResponse> AddBenevolenceTransfer(AddBenevolenceTransferRequest request)
        {
            request.CheckNotNull(nameof(request));

            var command = new CreateBenevolenceTransferCommand(
                GuidUtil.NewSequentialId(),
                request.Id,
                DateTime.Now.ToSerialNumber(),
                BenevolenceTransferType.SystemOp,
                BenevolenceTransferStatus.Placed,
                request.Amount,
                0,
                request.Direction,
                request.Remark);
            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }
        /// <summary>
        /// 所有的提现申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("WalletAdmin/WithdrawApplysListPage")]
        public WithdrawApplysListPageResponse WithdrawApplysListPage(WithdrawApplysListPageRequest request)
        {
            request.CheckNotNull(nameof(request));

            var pageSize = 20;
            var withdrawApplylogs = _walletQueryService.WithdrawApplyLogs().Where(x=>x.Status==request.Status);
            var total = withdrawApplylogs.Count();
            if (!request.Name.IsNullOrEmpty())
            {
                withdrawApplylogs = withdrawApplylogs.Where(x => x.BankOwner.Contains(request.Name)).OrderByDescending(x => x.CreatedOn).Skip(pageSize * (request.Page - 1)).Take(pageSize);
                total = withdrawApplylogs.Count();
            }
            else
            {
                withdrawApplylogs = withdrawApplylogs.OrderByDescending(x => x.CreatedOn).Skip(pageSize * (request.Page - 1)).Take(pageSize);
            }

            return new WithdrawApplysListPageResponse
            {
                Total = total,
                WithdrawApplys = withdrawApplylogs.Select(x => new WithdrawApply
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    BankName = x.BankName,
                    BankNumber = x.BankNumber,
                    BankOwner = x.BankOwner,
                    CreatedOn = x.CreatedOn,
                    Remark = x.Remark,
                    Status = x.Status.ToDescription(),
                    WalletId = x.WalletId
                }).ToList()
            };
        }

        /// <summary>
        /// 更改申请状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("WalletAdmin/ChangeWithdrawApplyStatus")]
        public async Task<BaseApiResponse> ChangeWithdrawApplyStatus(ChangeWithdrawApplyStatusRequest request)
        {
            request.CheckNotNull(nameof(request));
            request.WalletId.CheckNotEmpty(nameof(request.WalletId));

            var command = new ChangeWithdrawStatusCommand(
                request.WithdrawApplyId,
                request.Status,
                request.Remark)
            {
                AggregateRootId = request.WalletId
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }
        /// <summary>
        /// 审核线下充值
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("WalletAdmin/ChangeRechargeApplyStatus")]
        public async Task<BaseApiResponse> ChangeRechargeApplyStatus(ChangeRechargeApplyStatusRequest request)
        {
            request.CheckNotNull(nameof(request));
            request.WalletId.CheckNotEmpty(nameof(request.WalletId));

            var command = new ChangeRechargeStatusCommand(
                request.RechargeApplyId,
                request.Status,
                request.Remark)
            {
                AggregateRootId = request.WalletId
            };

            var result = await ExecuteCommandAsync(command);
            if (!result.IsSuccess())
            {
                return new BaseApiResponse { Code = 400, Message = "命令没有执行成功：{0}".FormatWith(result.GetErrorMessage()) };
            }
            return new BaseApiResponse();
        }

        /// <summary>
        /// 线下充值申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("WalletAdmin/RechargeApplysListPage")]
        public RechargeApplysListPageResponse RechargeApplysListPage(RechargeApplysListPageRequest request)
        {
            request.CheckNotNull(nameof(request));

            var pageSize = 20;
            var rechargeApplylogs = _walletQueryService.RechargeApplyLogs().Where(x=>x.Status==request.Status);
            var total = rechargeApplylogs.Count();
            if (!request.Name.IsNullOrEmpty())
            {
                rechargeApplylogs = rechargeApplylogs.Where(x => x.BankOwner.Contains(request.Name)).OrderByDescending(x => x.CreatedOn).Skip(pageSize * (request.Page - 1)).Take(pageSize);
                total = rechargeApplylogs.Count();
            }
            else
            {
                rechargeApplylogs = rechargeApplylogs.OrderByDescending(x => x.CreatedOn).Skip(pageSize * (request.Page - 1)).Take(pageSize);
            }
            return new RechargeApplysListPageResponse
            {
                Total = total,
                RechargeApplys = rechargeApplylogs.Select(x => new RechargeApply
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Pic = x.Pic,
                    BankName = x.BankName,
                    BankNumber = x.BankNumber,
                    BankOwner = x.BankOwner,
                    CreatedOn = x.CreatedOn,
                    Remark = x.Remark,
                    Status = x.Status.ToDescription(),
                    WalletId = x.WalletId
                }).ToList()
            };
        }
        #endregion

        #region 私有方法
        private Task<AsyncTaskResult<CommandResult>> ExecuteCommandAsync(ICommand command, int millisecondsDelay = 50000)
        {
            return _commandService.ExecuteAsync(command, CommandReturnType.EventHandled).TimeoutAfter(millisecondsDelay);
        }
        #endregion
    }
}
