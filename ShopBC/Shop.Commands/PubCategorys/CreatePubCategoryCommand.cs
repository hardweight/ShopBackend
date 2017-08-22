using ENode.Commanding;
using System;

namespace Shop.Commands.PubCategorys
{
    public class CreatePubCategoryCommand : Command<Guid>
    {
        public Guid ParentId { get; private set; }
        public string Name { get; private set; }
        public string Thumb { get; private set; }
        public int Sort { get; private set; }

        public CreatePubCategoryCommand() { }
        public CreatePubCategoryCommand(Guid id,Guid parentId,string name,string thumb,int sort):base(id)
        {
            ParentId = parentId;
            Name = name;
            Thumb = thumb;
            Sort = sort;
        }
    }
}
