using ENode.Commanding;
using System;

namespace Shop.Commands.Users
{
    /// <summary>
    /// 接受子 的善心分成
    /// </summary>
    public class AcceptChildBenevolenceCommand:Command<Guid>
    {
        public Guid ParentId { get;private set; }
        public decimal Amount { get; private set; }
        public int Level { get; private set; }


        public AcceptChildBenevolenceCommand() { }
        public AcceptChildBenevolenceCommand(Guid parentId,decimal amount,int level):base(parentId)
        {
            Amount = amount;
            Level = level;
        }
    }
}
