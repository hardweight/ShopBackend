using ECommon.Components;
using ECommon.IO;
using ENode.Commanding;
using ENode.Infrastructure;
using Shop.Commands.Wallets;
using Shop.Commands.Wallets.BenevolenceTransfers;
using Shop.Domain.Events.Wallets;
using Shop.Domain.Events.Wallets.BenevolenceTransfers;
using System.Threading.Tasks;

namespace Shop.ProcessManagers
{
    [Component]
    public class BenevolenceTransferProcessManager : 
        IMessageHandler<BenevolenceTransferCreatedEvent>,
        IMessageHandler<NewBenevolenceTransferEvent>  //钱包接受了新的现金记录  更新现金记录状态为成功
    {
        private ICommandService _commandService;

        public BenevolenceTransferProcessManager(ICommandService commandService)
        {
            _commandService = commandService;
        }

        public Task<AsyncTaskResult> HandleAsync(BenevolenceTransferCreatedEvent evnt)
        {
            return _commandService.SendAsync(new AcceptNewBenevolenceTransferCommand(evnt.WalletId, evnt.AggregateRootId));
        }

        public Task<AsyncTaskResult> HandleAsync(NewBenevolenceTransferEvent evnt)
        {
            return _commandService.SendAsync(
                new SetBenevolenceTransferSuccessCommand(evnt.TransferId)
                );
        }
    }
}
