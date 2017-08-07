namespace Shop.Commands.Wallets.BenevolenceTransfers
{
    public enum BenevolenceTransferType
    {
        /// <summary>
        /// 购物奖励 & 5倍善心
        /// </summary>
        ShoppingAward = 0,
        /// <summary>
        /// 商家售货奖励
        /// </summary>
        StoreAward = 1,
        /// <summary>
        /// 推荐用户奖励 一度 二度
        /// </summary>
        RecommendUserAward = 2,
        /// <summary>
        /// 推荐商家奖励
        /// </summary>
        RecommendStoreAward = 3,
        /// <summary>
        /// 联盟分成
        /// </summary>
        UnionAward = 4,
        /// <summary>
        /// 转账
        /// </summary>
        Transfer = 5,
        /// <summary>
        /// 善心激励
        /// </summary>
        Incentive = 6
    }
    public enum BenevolenceTransferStatus
    {
        Placed = 0,//提交
        Success = 1,//交易成功
        Refused = 2//拒绝-失败
    }
}
