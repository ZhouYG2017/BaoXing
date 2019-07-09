using System.Web.Http;
using WebActivatorEx;
using DoNet.WebAPI;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace DoNet.WebAPI
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "DoNet.WebAPI");
                    c.IncludeXmlComments(GetXmlCommentsPath());
                    c.OperationFilter<HttpHeaderFilter>();
                })
                .EnableSwaggerUi(c =>
                {
                });
        }
        private static string GetXmlCommentsPath()
        {
            return string.Format("{0}/bin/DoNet.WebAPI.xml", System.AppDomain.CurrentDomain.BaseDirectory);

        }
    }
}
