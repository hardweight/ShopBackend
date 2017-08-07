using System.Net;
using System.Collections.Generic;
using ECommon.Socketing;
using ENode.Configurations;
using ENode.EQueue;
using ENode.Eventing;
using ENode.Infrastructure;
using EQueue.Clients.Consumers;
using EQueue.Clients.Producers;
using EQueue.Configurations;
using Shop.Common;
using ENode.Commanding;

namespace Shop.ProcessorHost
{
    public static class ENodeExtensions
    {
        private static CommandService _commandService;

        private static CommandConsumer _commandConsumer;
        private static ApplicationMessagePublisher _applicationMessagePublisher;
        private static ApplicationMessageConsumer _applicationMessageConsumer;

        private static DomainEventPublisher _domainEventPublisher;
        private static DomainEventConsumer _eventConsumer;

        private static PublishableExceptionPublisher _exceptionPublisher;
        private static PublishableExceptionConsumer _exceptionConsumer;

        public static ENodeConfiguration UseEQueue(this ENodeConfiguration enodeConfiguration)
        {
            var configuration = enodeConfiguration.GetCommonConfiguration();

            configuration.RegisterEQueueComponents();

            var producerSetting = new ProducerSetting
            {
                NameServerList = new List<IPEndPoint> { new IPEndPoint(SocketUtils.GetLocalIPV4(), ConfigSettings.NameServerPort) }
            };
            var consumerSetting = new ConsumerSetting
            {
                NameServerList = new List<IPEndPoint> { new IPEndPoint(SocketUtils.GetLocalIPV4(), ConfigSettings.NameServerPort) }
            };

            _domainEventPublisher = new DomainEventPublisher(producerSetting);
            configuration.SetDefault<IMessagePublisher<DomainEventStreamMessage>, DomainEventPublisher>(_domainEventPublisher);

            _commandService = new CommandService(null, producerSetting);
            configuration.SetDefault<ICommandService, CommandService>(_commandService);

            _commandConsumer = new CommandConsumer("ShopCommandConsumerGroup", consumerSetting).Subscribe(Topics.ShopCommandTopic);
            _eventConsumer = new DomainEventConsumer("ShopEventConsumerGroup", consumerSetting).Subscribe(Topics.ShopDomainEventTopic);

            _applicationMessageConsumer = new ApplicationMessageConsumer("ShopMessageConsumerGroup", consumerSetting)
           .Subscribe(Topics.ShopApplicationMessageTopic)
           .Subscribe(Topics.PaymentApplicationMessageTopic);

            _applicationMessagePublisher = new ApplicationMessagePublisher(producerSetting);
            
            _exceptionPublisher = new PublishableExceptionPublisher(producerSetting);


            configuration.SetDefault<IMessagePublisher<IApplicationMessage>, ApplicationMessagePublisher>(_applicationMessagePublisher);
            configuration.SetDefault<IMessagePublisher<IPublishableException>, PublishableExceptionPublisher>(_exceptionPublisher);

            _exceptionConsumer = new PublishableExceptionConsumer("ShopExceptionConsumerGroup", consumerSetting).Subscribe(Topics.ShopExceptionTopic);

            return enodeConfiguration;
        }


        public static ENodeConfiguration StartEQueue(this ENodeConfiguration enodeConfiguration)
        {
            _commandService.Start();
            _domainEventPublisher.Start();
            _exceptionConsumer.Start();
            _eventConsumer.Start();
            _commandConsumer.Start();
            _applicationMessagePublisher.Start();
            _exceptionPublisher.Start();
            _applicationMessageConsumer.Start();

            return enodeConfiguration;
        }

        public static ENodeConfiguration ShutdownEQueue(this ENodeConfiguration enodeConfiguration)
        {
            _commandService.Shutdown();
            _applicationMessagePublisher.Shutdown();
            _domainEventPublisher.Shutdown();
            _exceptionPublisher.Shutdown();
            _commandConsumer.Shutdown();
            _eventConsumer.Shutdown();
            _exceptionConsumer.Shutdown();
            _applicationMessageConsumer.Shutdown();
            return enodeConfiguration;
        }
    }
}
