using ECommon.Components;
using ECommon.Configurations;
using ECommon.Logging;
using ENode.Configurations;
using Shop.Common;
using Shop.TimerTask.Jobs.OrderGoodses;
using Shop.TimerTask.Jobs.Orders;
using Shop.TimerTask.Jobs.WithdrawClearWeekAmount;
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
            try
            {
                InitializeENode();
                //启动定时任务
                StartTimerTasks();
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
            _ecommonConfiguration = ECommonConfiguration
                .Create()
                .UseAutofac()
                .RegisterCommonComponents()
                .UseLog4Net()
                .UseJsonNet()
                .RegisterUnhandledExceptionHandler();

            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create(typeof(Bootstrap).FullName);
            _logger.Info("ECommon 初始化成功.");
        }

        private static void StartTimerTasks()
        {
            OrderJobScheduler.Start();//预订单30分钟付款到期
            WithdrawClearWeekAmountJobScheduler.Start();//每周日晚上清空本周提现金额
            //OrderGoodsJobScheduler.Start();//商品服务自动过期服务
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
                .InitializeBusinessAssemblies(assemblies);
            //应该将组件注入到AOC容器
            _logger.Info("ENode initialized.");
        }
    }
}
