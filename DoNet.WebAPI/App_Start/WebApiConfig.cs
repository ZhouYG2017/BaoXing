using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DoNet.WebAPI.App_Start;

namespace DoNet.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();
            config.Filters.Add(new CustomerErrorAttribute());
            config.Filters.Add(new ValidModelAttributepublic());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
