using DoNet.Application;
using DoNet.Domain.Entity;
using DoNet.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DoNet.WebAPI.Controllers
{
    /// <summary>
    /// 客户
    /// </summary>
    public class CustomerController : ApiController
    {
        private CustomerApp tblCustomerApp =new CustomerApp();

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public Response<LoginResponse> Login(ParamLogin paramLogin)
        {
            return tblCustomerApp.Login(paramLogin);
        }
    }
}
