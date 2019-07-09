using Autofac;
using Autofac.Integration.WebApi;
using DoNet.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace DoNet.WebAPI
{
    public class AutoFacConfig
    {
        public static void InitAutoFac()
        {
            var builder = new ContainerBuilder();
            HttpConfiguration config = GlobalConfiguration.Configuration;

            SetupResolveRules(builder);
            
            //注册所有的ApiControllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();

            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver((IContainer)container);

        }

        private static void SetupResolveRules(ContainerBuilder builder)
        {
            //WebAPI只用引用的接口，不用引用实现的dll。
            //如需加载实现的程序集，将dll拷贝到bin目录下即可，不用引用dll
            var application = Assembly.Load("DoNet.Application");
            var domain = Assembly.Load("DoNet.Domain");
            var repository = Assembly.Load("DoNet.Repository");
            
            builder.RegisterAssemblyTypes(application)
              .Where(t => t.Name.EndsWith("App"))
              .SingleInstance()
              .PropertiesAutowired();

            builder.RegisterAssemblyTypes(domain, repository)
              .Where(t => t.Name.EndsWith("Repository"))
              .AsImplementedInterfaces()
              .SingleInstance()
              .PropertiesAutowired();
        }
    }
}