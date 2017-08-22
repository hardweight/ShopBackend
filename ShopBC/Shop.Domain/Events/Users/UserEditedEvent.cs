using ENode.Eventing;
using System;

namespace Shop.Domain.Events.Users
{
    [Serializable]
    public class UserEditedEvent:DomainEvent<Guid>
    {
        public string NickName { get; set; }
        public string Gender { get; set; }

        public UserEditedEvent() { }
        public UserEditedEvent(string nickName,string gender)
        {
            NickName = nickName;
            Gender = gender;
        }
    }
}
