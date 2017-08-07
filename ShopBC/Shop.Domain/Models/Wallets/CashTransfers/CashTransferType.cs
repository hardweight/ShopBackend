using System.ComponentModel;

namespace Shop.Domain.Models.Wallets.CashTransfers
{
    public enum CashTransferType
    {
        [Description("充值")]
        Charge = 0,
        [Description("提现")]
        Withdraw = 1,
        [Description("转账")]
        Transfer = 2,
        [Description("善心激励")]
        Incentive = 3,
        [Description("消费")]
        Shopping =4
    }
    public enum CashTransferStatus
    {
        [Description("提交")]
        Placed = 0,
        [Description("成功")]
        Success = 1,
        [Description("失败")]
        Refused = 2
    }
    

}
