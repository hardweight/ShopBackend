using System.ComponentModel;

namespace Shop.Common.Enums
{
    public enum GoodsStatus
    {
        [Description("待审核")]
        UnVerify=0,
        [Description("已审核")]
        Verifyed=1
    }
}
