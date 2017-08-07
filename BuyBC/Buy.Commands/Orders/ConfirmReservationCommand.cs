using ENode.Commanding;
using System;

namespace Buy.Commands.Orders
{
    /// <summary>
    /// 订单 确认预定成功与否 命令
    /// </summary>
    [Serializable]
    public class ConfirmReservationCommand : Command<Guid>
    {
        public bool IsReservationSuccess { get; set; }

        public ConfirmReservationCommand() { }
        /// <summary>
        /// 确认预定成功与否的命令
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="isReservationSuccess"></param>
        public ConfirmReservationCommand(Guid orderId, bool isReservationSuccess) : base(orderId)
        {
            IsReservationSuccess = isReservationSuccess;
        }
    }
}
