using System.ComponentModel;

namespace Shop.Common.Enums
{
    public enum WithdrawApplyStatus
    {
        [Description("提交")]
        Placed,
        [Description("成功")]
        Success,
        [Description("拒绝")]
        Rejected
    }

    public enum RechargeApplyStatus
    {
        [Description("提交")]
        Placed,
        [Description("成功")]
        Success,
        [Description("拒绝")]
        Rejected
    }

    public enum WalletDirection
    {
        [Description("进账")]
        In = 0,//进账
        [Description("出账")]
        Out = 1//出账
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
        Shopping = 4,
        [Description("系统操作")]
        SystemOp=5,
        /// <summary>
        /// 退款
        /// </summary>
        [Description("退款")]
        Refund=6,

        [Description("店铺售货")]
        StoreSell=7
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


    public enum BenevolenceTransferType
    {
        [Description("购物奖励")]
        ShoppingAward = 0,
        [Description("商家奖励")]
        StoreAward = 1,
        [Description("推荐奖励")]
        RecommendUserAward = 2,
        [Description("推荐商家奖励")]
        RecommendStoreAward = 3,
        [Description("联盟分成")]
        UnionAward = 4,
        [Description("转账")]
        Transfer = 5,
        [Description("善心激励")]
        Incentive = 6,
        [Description("系统操作")]
        SystemOp = 7
    }

    public enum BenevolenceTransferStatus
    {
        [Description("提交")]
        Placed = 0,
        [Description("成功")]
        Success = 1,
        [Description("失败")]
        Refused = 2
    }
}
