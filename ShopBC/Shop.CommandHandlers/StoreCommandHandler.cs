using ECommon.Components;
using ENode.Commanding;
using Shop.Commands.Stores;
using Shop.Domain.Models.Stores;

namespace Shop.CommandHandlers
{
    [Component]
    public class StoreCommandHandler:
        ICommandHandler<CreateStoreCommand>,
        ICommandHandler<UpdateStoreCommand>,
        ICommandHandler<AcceptNewStoreOrderCommand>,
        ICommandHandler<SetAccessCodeCommand>
    {

        public void Handle(ICommandContext context, CreateStoreCommand command)
        {
            //创建聚合跟对象
            var store = new Store(command.AggregateRootId, command.UserId, new StoreInfo(
                command.AccessCode,
                command.Name,
                command.Description,
                command.Region,
                command.Address
                ),
                new SubjectInfo(
                    command.SubjectName,
                    command.SubjectNumber,
                    command.SubjectPic));
            //添加到上下文
            context.Add(store);
        }
        public void Handle(ICommandContext context, UpdateStoreCommand command)
        {
            context.Get<Store>(command.AggregateRootId).Update(new StoreEditableInfo (
                command.Name,
                command.Description,
                command.Address
            ));
        }
        public void Handle(ICommandContext context, AcceptNewStoreOrderCommand command)
        {
            context.Get<Store>(command.AggregateRootId).AcceptNewOrder(context.Get<StoreOrder>(command.StoreOrderId));
        }

        public void Handle(ICommandContext context, SetAccessCodeCommand command)
        {
            context.Get<Store>(command.AggregateRootId).SetAccessCode(command.AccessCode);
        }
    }
}
