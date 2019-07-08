using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace DoNet.WebAPI
{
    /// <summary>
    /// 自定义授权过滤器
    /// </summary>
    public class CustomerAuthorizationAttribute : AuthorizationFilterAttribute
    {
    }
}