using System;
using System.ComponentModel;

namespace Shop.ReadModel.Wallets.Dtos
{
    public class CashTransfer
    {
        public Guid Id { get; set; }
        private Guid WalletId;//钱包Id
        public string Number { get; set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
        public CashTransferType Type { get; set; }
        public CashTransferStatus Status { get; set; }
        public WalletDirection Direction { get; private set; }
        public DateTime CreatedOn { get; set; }
        public string Remark { get; private set; }
    }

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
        Shopping = 4
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
    public enum WalletDirection
    {
        [Description("进账")]
        In = 0,//进账
        [Description("出账")]
        Out = 1//出账
    }
}
