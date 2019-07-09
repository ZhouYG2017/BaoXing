using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;
using DoNet.Code;
using DoNet.Domain.Entity;

namespace DoNet.WebAPI
{
    /// <summary>
    /// 自定义授权过滤器
    /// </summary>
    public class CustomerAuthorizationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var content = actionContext.Request.Properties["MS_HttpContext"] as HttpContextBase;
            var token = content.Request.Headers["Token"]; 
            if (token.IsEmpty())
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Code = "401",
                    Message = "Token为空"
                });
            }
            else
            {
                var entity = JwtHelper.GetJwtDecode<CustomerEntity>(token);
                base.OnAuthorization(actionContext);
            }
        }
    }
}