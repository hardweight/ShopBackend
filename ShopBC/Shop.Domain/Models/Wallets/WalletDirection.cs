using System.ComponentModel;

namespace Shop.Domain.Models.Wallets
{
    public enum WalletDirection
    {
        [Description("进账")]
        In = 0,//进账
        [Description("出账")]
        Out = 1//出账
    }
}
