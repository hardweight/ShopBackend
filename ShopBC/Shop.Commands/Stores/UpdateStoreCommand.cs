using System;
using ENode.Commanding;

namespace Shop.Commands.Stores
{
    public class UpdateStoreCommand: Command<Guid>
    {
        public string Name { get; private set; }
        public string Description { get;private  set; }
        public string Address { get;private set; }

        public UpdateStoreCommand() { }
        public UpdateStoreCommand(string name,string description,string address)
        {
            Name = name;
            Description = description;
            Address = address;
        }
    }
}
