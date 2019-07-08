using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace DoNet.WebAPI.App_Start
{
    public class AutoFacConfig
    {
        public static void InitAutoFac()
        {
            //得到你的HttpConfiguration.
            var configuration = GlobalConfiguration.Configuration;
            var builder = new ContainerBuilder();

            //注册控制器
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly())
                .PropertiesAutowired();
            //可选：注册Autofac过滤器提供商.
            builder.RegisterWebApiFilterProvider(configuration);

            var application = Assembly.Load("DoNet.Application");
            var iRepository = Assembly.Load("DoNet.Domain");
            var repository = Assembly.Load("DoNet.Repository");

            builder.RegisterAssemblyTypes(application)
                .Where(t => t.Name.EndsWith("App"))
                .AsImplementedInterfaces()
                .PropertiesAutowired();

            //builder.RegisterAssemblyTypes(iRepository, repository)
            //    .Where(t => t.Name.EndsWith("Repository"))
            //    .AsImplementedInterfaces()
            //    .PropertiesAutowired();


            IContainer container = builder.Build();
            //将依赖关系解析器设置为Autofac。
            var resolver = new AutofacWebApiDependencyResolver(container);
            configuration.DependencyResolver = resolver;
        }
    }
}