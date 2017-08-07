using ECommon.Components;
using ENode.Commanding;
using ENode.Infrastructure;
using Shop.Commands.Goodses;
using Shop.Commands.Goodses.Specifications;
using Shop.Domain.Models.Comments;
using Shop.Domain.Models.Goodses;
using System.Linq;

namespace Shop.CommandHandlers
{
    [Component]
    public class ConferenceCommandHandler :
        ICommandHandler<CreateGoodsCommand>,
        ICommandHandler<UpdateGoodsCommand>,
        ICommandHandler<PublishGoodsCommand>,
        ICommandHandler<UnpublishGoodsCommand>,
        ICommandHandler<AddSpecificationCommand>,
        ICommandHandler<UpdateSpecificationCommand>,
        ICommandHandler<MakeSpecificationReservationCommand>,
        ICommandHandler<CommitSpecificationReservationCommand>,
        ICommandHandler<CancelSpecificationReservationCommand>,
        ICommandHandler<AcceptNewCommentCommand>
    {
        private readonly ILockService _lockService;

        public ConferenceCommandHandler(ILockService lockService)
        {
            _lockService = lockService;
        }

        public void Handle(ICommandContext context, CreateGoodsCommand command)
        {
            //创建聚合跟对象
            var goods = new Goods(command.AggregateRootId,command.StoreId, new GoodsInfo(
                command.Name,
                command.Description,
                command.Pics,
                command.Price,
                command.OriginalPrice,
                command.Stock,
                new SurrenderInfo(command.Price,command.SurrenderPersent),
                command.SellOut,
                command.IsPayOnDelivery,
                command.IsInvoice,
                command.Is7SalesReturn,
                command.Sort));
            //添加到上下文
            context.Add(goods);
        }
        public void Handle(ICommandContext context, UpdateGoodsCommand command)
        {
            context.Get<Goods>(command.AggregateRootId).Update(new GoodsEditableInfo(
                command.Name,
                command.Description,
                command.Pics,
                command.Price,
                command.OriginalPrice,
                command.Stock,
                command.SurrenderPersent,
                command.SellOut,
                command.IsPayOnDelivery,
                command.IsInvoice,
                command.Is7SalesReturn,
                command.Sort));
        }
        public void Handle(ICommandContext context, PublishGoodsCommand command)
        {
            context.Get<Goods>(command.AggregateRootId).Publish();
        }
        public void Handle(ICommandContext context, UnpublishGoodsCommand command)
        {
            context.Get<Goods>(command.AggregateRootId).Unpublish();
        }
        public void Handle(ICommandContext context, AddSpecificationCommand command)
        {
            context.Get<Goods>(command.AggregateRootId).AddSpecification(
                new Domain.Models.Goodses.Specifications.SpecificationInfo(
                    command.Name,
                    command.Thumb,
                    command.Price,
                    command.OriginalPrice,
                    command.Number,
                    command.BarCode
                ),
                command.Stock);
        }
       
        public void Handle(ICommandContext context, UpdateSpecificationCommand command)
        {
            context.Get<Goods>(command.AggregateRootId).UpdateSpecification(
                command.SpecificationId,
                new Domain.Models.Goodses.Specifications.SpecificationInfo(
                    command.Name, 
                    command.Thumb,
                    command.Price,
                    command.OriginalPrice,
                    command.Number,
                    command.BarCode),command.Stock);
        }
        /// <summary>
        /// 处理商品预定指令
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        public void Handle(ICommandContext context, MakeSpecificationReservationCommand command)
        {
            context.Get<Goods>(command.AggregateRootId).MakeReservation(
                command.ReservationId, 
                command.Specifications.Select(x => new ReservationItem(x.SpecificationId, x.Quantity)).ToList());
        }
        public void Handle(ICommandContext context, CommitSpecificationReservationCommand command)
        {
            context.Get<Goods>(command.AggregateRootId).CommitReservation(command.ReservationId);
        }
        public void Handle(ICommandContext context, CancelSpecificationReservationCommand command)
        {
            context.Get<Goods>(command.AggregateRootId).CancelReservation(command.ReservationId);
        }
        public void Handle(ICommandContext context, AcceptNewCommentCommand command)
        {
            context.Get<Goods>(command.AggregateRootId).AcceptNewComment(context.Get<Comment>(command.CommentId));
        }
    }
}
