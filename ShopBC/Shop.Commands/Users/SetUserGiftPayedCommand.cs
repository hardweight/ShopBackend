using ENode.Commanding;
using System;

namespace Shop.Commands.Users
{
    public class SetUserGiftPayedCommand:Command<Guid>
    {
        public Guid UserGiftId { get; private set; }

        public SetUserGiftPayedCommand() { }
        public SetUserGiftPayedCommand(Guid userId,Guid userGiftId):base(userId)
        {
            UserGiftId = userGiftId;
        }
    }
}
