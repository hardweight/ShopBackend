using ENode.Commanding;
using System;

namespace Shop.Commands.Categorys
{
    public class CreateCategoryCommand : Command<Guid>
    {
        public Guid ParentId { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string Thumb { get; private set; }

        public CreateCategoryCommand() { }
        public CreateCategoryCommand(Guid id,Guid parentId,string name,string url,string thumb):base(id)
        {
            ParentId = parentId;
            Name = name;
            Url = url;
            Thumb = thumb;
        }
    }
}
