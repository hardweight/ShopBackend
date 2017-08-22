using ECommon.Components;
using ENode.Commanding;
using Shop.Commands.PubCategorys;
using Shop.Domain.Models.PubCategorys;
using System;

namespace Shop.CommandHandlers
{
    [Component]
    public class PubCategoryCommandHandler :
        ICommandHandler<CreatePubCategoryCommand>,
        ICommandHandler<UpdatePubCategoryCommand>
    {
        public void Handle(ICommandContext context, CreatePubCategoryCommand command)
        {
            PubCategory parent = null;
            if (command.ParentId != Guid.Empty)
            {
                parent = context.Get<PubCategory>(command.ParentId);
            }
            var category = new PubCategory(
                command.AggregateRootId,
                parent, 
                command.Name,
                command.Thumb,
                command.Sort);
            
            //将领域对象添加到上下文中
            context.Add(category);
        }

        public void Handle(ICommandContext context, UpdatePubCategoryCommand command)
        {
            context.Get<PubCategory>(command.AggregateRootId).UpdateCategory(
                command.Name,
                command.Thumb,
                command.Sort);
        }
    }
}
