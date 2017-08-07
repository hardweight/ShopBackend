using System;
using System.Net;
using Shop.Common;
using ECommon.Components;
using ECommon.Configurations;
using ECommon.Socketing;
using ECommon.Logging;
using EQueue.NameServer;
using EQueue.Configurations;
using ECommonConfiguration = ECommon.Configurations.Configuration;

namespace Shop.MessageNameServer
{
    /// <summary>
    /// 服务容器
    /// </summary>
    public class Bootstrap
    {
        private static ILogger _logger;
        private static ECommonConfiguration _configuration;
        private static NameServerController _nameServer;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            ConfigSettings.Initialize();
            InitializeECommon();
            try
            {
                InitializeNameServer();
            }
            catch (Exception ex)
            {
                _logger.Error("NameServer 初始化失败.", ex);
                throw;
            }
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        public static void Start()
        {
            try
            {
                _nameServer.Start();
            }
            catch (Exception ex)
            {
                _logger.Error("NameServer 启动失败.", ex);
                throw;
            }
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public static void Stop()
        {
            try
            {
                if (_nameServer != null)
                {
                    _nameServer.Shutdown();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("NameServer 停止失败", ex);
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
            //从IOC 容器获取一个ILoggerFactory实例 创建ILogger实例
            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create(typeof(Bootstrap).FullName);
            _logger.Info("ECommon 初始化成功.");
        }
        /// <summary>
        /// 初始化NameServer
        /// </summary>
        private static void InitializeNameServer()
        {
            _configuration.RegisterEQueueComponents();
            var setting = new NameServerSetting()
            {
                BindingAddress = new IPEndPoint(SocketUtils.GetLocalIPV4(), ConfigSettings.NameServerPort)
            };
            _nameServer = new NameServerController(setting);
            _logger.Info("NameServer 初始化成功.");
        }
    }
}
