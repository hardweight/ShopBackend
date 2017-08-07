using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Shop.Common;
using ECommon.Components;
using ECommon.Configurations;
using ECommon.Socketing;
using ECommon.Logging;
using EQueue.Broker;
using EQueue.Configurations;
using EQueue.Utils;
using ECommonConfiguration = ECommon.Configurations.Configuration;

namespace Shop.MessageBroker
{
    /// <summary>
    /// 服务器容器
    /// </summary>
    public class Bootstrap
    {
        private static ILogger _logger;
        private static ECommonConfiguration _configuration;

        private static BrokerController _broker;

        public static void Initialize()
        {
            ConfigSettings.Initialize();
            InitializeECommon();
            try
            {
                InitializeEQueue();
            }
            catch (Exception ex)
            {
                _logger.Error("EQueue 初始化失败.", ex);
                throw;
            }
        }
        public static void Start()
        {
            try
            {
                _broker.Start();
            }
            catch (Exception ex)
            {
                _logger.Error("Broker 启动失败.", ex);
                throw;
            }
        }
        public static void Stop()
        {
            try
            {
                if (_broker != null)
                {
                    _broker.Shutdown();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Broker 停止失败.", ex);
                throw;
            }
        }
        /// <summary>
        /// 初始化ECommon
        /// </summary>
        private static void InitializeECommon()
        {
            _configuration = ECommonConfiguration
                .Create()
                .UseAutofac()
                .RegisterCommonComponents()
                .UseLog4Net()
                .UseJsonNet()
                .RegisterUnhandledExceptionHandler();
            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create(typeof(Bootstrap).FullName);
            _logger.Info("ECommon 初始化成功.");
        }
        /// <summary>
        /// 初始化队列
        /// </summary>
        private static void InitializeEQueue()
        {
            _configuration.RegisterEQueueComponents();
            //EQueue配置
            var storePath = ConfigurationManager.AppSettings["equeueStorePath"];
            var setting = new BrokerSetting(false, storePath);
            setting.NameServerList = new List<IPEndPoint> { new IPEndPoint(SocketUtils.GetLocalIPV4(), ConfigSettings.NameServerPort) };
            setting.BrokerInfo.BrokerName = "ShopBroker";
            setting.BrokerInfo.GroupName = "ShopGroupName";
            setting.BrokerInfo.ProducerAddress = new IPEndPoint(SocketUtils.GetLocalIPV4(), ConfigSettings.BrokerProducerPort).ToAddress();
            setting.BrokerInfo.ConsumerAddress = new IPEndPoint(SocketUtils.GetLocalIPV4(), ConfigSettings.BrokerConsumerPort).ToAddress();
            setting.BrokerInfo.AdminAddress = new IPEndPoint(SocketUtils.GetLocalIPV4(), ConfigSettings.BrokerAdminPort).ToAddress();
            //创建broker
            _broker = BrokerController.Create(setting);
            _logger.Info("EQueue，初始化成功");
        }
    }
}
