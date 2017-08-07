using System.ComponentModel;

namespace Shop.Commands.Wallets.CashTransfers
{
    public enum CashTransferType
    {
        /// <summary>
        /// 充值
        /// </summary>
        [Description("充值")]
        Charge = 0,
        /// <summary>
        /// 提现
        /// </summary>
        [Description("提现")]
        Withdraw = 1,
        /// <summary>
        /// 转账
        /// </summary>
        [Description("转账")]
        Transfer = 2,
        /// <summary>
        /// 善心激励
        /// </summary>
        [Description("善心激励")]
        Incentive = 3,
        /// <summary>
        /// 消费
        /// </summary>
        [Description("消费")]
        Shopping =4
    }
    public enum CashTransferStatus
    {
        /// <summary>
        /// 提交
        /// </summary>
        [Description("提交")]
        Placed = 0,
        /// <summary>
        /// 交易成功
        /// </summary>
        [Description("交易成功")]
        Success = 1,
        /// <summary>
        /// 拒绝-失败
        /// </summary>
        [Description("失败")]
        Refused = 2
    }
    
}
