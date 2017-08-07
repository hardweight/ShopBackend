using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buy.Domain.Orders.Models
{
    public enum OrderStatus
    {
        Placed = 1,                //订单已生成
        ReservationSuccess = 2,    //预定已成功（下单已成功）
        ReservationFailed = 3,     //预定已失败（下单失败）
        PaymentSuccess = 4,        //付款已成功
        PaymentRejected = 5,       //付款已拒绝
        Expired = 6,               //订单已过期
        Success = 7,               //交易已成功
        Closed = 8                 //订单已关闭
    }
}
