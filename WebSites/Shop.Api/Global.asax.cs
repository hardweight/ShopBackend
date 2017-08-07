using Autofac;
using Autofac.Integration.WebApi;
using ECommon.Autofac;
using ECommon.Components;
using ECommon.Configurations;
using ECommon.Logging;
using ENode.Configurations;
using Shop.Api.Extensions;
using Shop.Common;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Shop.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private ILogger _logger;
        private Configuration _ecommonConfiguration;
        private ENodeConfiguration _enodeConfiguration;

        protected void Application_Start()
        {
            ConfigSettings.Initialize();

            //AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);

            InitializeECommon();
            InitializeENode();
        }

        /// <summary>
        /// 初始化ECommon
        /// </summary>
        private void InitializeECommon()
        {
            _ecommonConfiguration = Configuration
                .Create()
                .UseAutofac()
                .RegisterCommonComponents()
                .UseLog4Net()
                .UseJsonNet()
                .RegisterUnhandledExceptionHandler();

            _logger = ObjectContainer.Resolve<ILoggerFactory>().Create(GetType().FullName);
            _logger.Info("ECommon initialized.");
        }
        /// <summary>
        /// 初始化ENode
        /// </summary>
        private void InitializeENode()
        {
            ConfigSettings.Initialize();

            var assemblies = new[]
            {
                Assembly.Load("Shop.Commands"),
                Assembly.Load("Shop.Domain"),
                Assembly.Load("Shop.ReadModel"),

                Assembly.Load("Buy.Commands"),
                Assembly.Load("Buy.Domain"),
                Assembly.Load("Buy.ReadModel"),

                Assembly.Load("Payments.Commands"),
                Assembly.Load("Payments.ReadModel"),
                Assembly.GetExecutingAssembly()
            };

            _enodeConfiguration = _ecommonConfiguration
                .CreateENode()
                .RegisterENodeComponents()//注册ENode的所有默认实现组件以及给定程序集中的所有标记了Component特性的组件到容器
                .RegisterBusinessComponents(assemblies)
                .UseEQueue()
                .InitializeBusinessAssemblies(assemblies)
                .StartEQueue();

            RegisterApiControllers();
            _logger.Info("ENode initialized.");
        }

        /// <summary>
        /// 注册apicontroller
        /// </summary>
        private void RegisterApiControllers()
        {
            // Mvc Register
            var webAssembly = Assembly.GetExecutingAssembly();
            var container = (ObjectContainer.Current as AutofacObjectContainer).Container;
            var builder = new ContainerBuilder();

            //WebApi Register
            builder.RegisterApiControllers(webAssembly);
            builder.Update(container);
            //Set the dependency resolver for Web API.
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

        }
    }
}
