using System;
using ENode.Commanding;
using Shop.Common.Enums;

namespace Shop.Commands.Stores
{
    public class UpdateStoreCommand: Command<Guid>
    {
        public string Name { get; private set; }
        public string Description { get;private  set; }
        public string Address { get;private set; }
        public StoreType Type { get; private set; }

        public UpdateStoreCommand() { }
        public UpdateStoreCommand(string name,string description,string address,StoreType type)
        {
            Name = name;
            Description = description;
            Address = address;
            Type = type;
        }
    }
}
