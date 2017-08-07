using System.ComponentModel;

namespace Shop.Domain.Models.Stores
{
    public enum StoreStatus
    {
        [Description("申请")]
        Apply,
        [Description("正常")]
        Normal,
        [Description("主体错误")]
        SubjectErr
    }
}
