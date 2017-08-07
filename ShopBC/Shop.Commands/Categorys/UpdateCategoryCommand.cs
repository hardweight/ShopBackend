using ENode.Commanding;
using System;

namespace Shop.Commands.Categorys
{
    public class UpdateCategoryCommand : Command<Guid>
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string Thumb { get; private set; }


        public UpdateCategoryCommand() { }
        public UpdateCategoryCommand(string name,string url,string thumb)
        {
            Name = name;
            Url = url;
            Thumb = thumb;
        }
    }
}
