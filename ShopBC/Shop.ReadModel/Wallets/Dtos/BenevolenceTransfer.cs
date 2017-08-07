using System;
using System.ComponentModel;

namespace Shop.ReadModel.Wallets.Dtos
{
    public class BenevolenceTransfer
    {
        public Guid Id { get; set; }
        private Guid WalletId;//钱包Id
        public string Number { get; set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
        public BenevolenceTransferType Type { get; set; }
        public BenevolenceTransferStatus Status { get; set; }
        public WalletDirection Direction { get; private set; }
        public DateTime CreatedOn { get; set; }
        public string Remark { get; private set; }
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
        Incentive = 6
    }

    public enum BenevolenceTransferStatus
    {
        [Description("提交")]
        Placed = 0,
        [Description("成功")]
        Success = 1,
        [Description("失败")]
        Refused = 2//拒绝-失败
    }
}
