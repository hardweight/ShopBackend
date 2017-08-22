using ECommon.Components;
using ECommon.Configurations;
using ECommon.Logging;
using ENode.Configurations;
using Shop.Common;
using Shop.TimerTask.Jobs.Orders;
using System;
using System.Reflection;
using ECommonConfiguration = ECommon.Configurations.Configuration;

namespace Shop.TimerTask
{
    public class Bootstrap
    {
        private static ILogger _logger;
        private static ECommonConfiguration _ecommonConfiguration;
        private static ENodeConfiguration _enodeConfiguration;
        /// <summary>
        /// 初始化服务
        /// </summary>
        public static void Initialize()
        {
            InitializeECommon();
            //启动定时任务
            OrderJobScheduler.Start();
            try
            {
                InitializeENode();
            }
            catch (Exception ex)
            {
                _logger.Error("ENode 初始化失败.", ex);
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
                _enodeConfiguration.StartEQueue();
            }
            catch (Exception ex)
            {
                _logger.Error("EQueue 启动失败.", ex);
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
                _enodeConfiguration.ShutdownEQueue();
            }
            catch (Exception ex)
            {
                _logger.Error("EQueue 停止失败.", ex);
                throw;
            }
        }

        /// <summary>
        /// 初始化ECommon
        /// </summary>
        private static void InitializeECommon()
        {
            _ecommonConfiguration = Configuration
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
        /// 初始化ENode
        /// </summary>
        private static void InitializeENode()
        {
            ConfigSettings.Initialize();

            var assemblies = new[]
            {
                Assembly.Load("Shop.Commands"),
                Assembly.Load("Shop.Domain"),
                Assembly.Load("Shop.ReadModel"),

                Assembly.GetExecutingAssembly()
            };

            var setting = new ConfigurationSetting(ConfigSettings.ENodeConnectionString);

            _enodeConfiguration = _ecommonConfiguration
                .CreateENode(setting)
                .RegisterENodeComponents()//注册ENode的所有默认实现组件以及给定程序集中的所有标记了Component特性的组件到容器
                .RegisterBusinessComponents(assemblies)
                .UseEQueue()
                .InitializeBusinessAssemblies(assemblies)
                .StartEQueue();

            //RegisterApiControllers();
            _logger.Info("ENode initialized.");
        }
    }
}
