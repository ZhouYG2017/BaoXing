using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;

namespace DoNet.WebAPI
{
    /// <summary>
    /// 自定义授权过滤器
    /// </summary>
    public class CustomerAuthorizationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            string token = actionContext.Request.Headers?.Where(x => x.Key == "token")?.Select(x=>x.Key)?.First();
            if (token==null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Code="401",
                    Message="token为空"
                });
            }
            base.OnAuthorization(actionContext);
        }
    }
}