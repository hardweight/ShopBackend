using ENode.Eventing;
using Shop.Domain.Models.Wallets;
using System;

namespace Shop.Domain.Events.Wallets
{
    [Serializable]
    public class WalletStatisticInfoChangedEvent:DomainEvent<Guid>
    {
        public WalletStatisticInfo StatisticInfo { get; private set; }

        private WalletStatisticInfoChangedEvent() { }
        public WalletStatisticInfoChangedEvent(WalletStatisticInfo statisticInfo)
        {
            StatisticInfo = statisticInfo;
        }
    }
}
