using DoNet.Application;
using DoNet.Domain.Entity;
using DoNet.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DoNet.Code;

namespace DoNet.WebAPI.Controllers
{
    /// <summary>
    /// 客户
    /// </summary>
    public class CustomerController : ApiController
    {
        public CustomerApp customerApp { get; set; }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="paramLogin"></param>
        /// <returns></returns>
        [HttpPost]
        public Response<LoginResponse> Login(ParamLogin paramLogin)
        {
            return customerApp.Login(paramLogin);
        }
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="paramAddCustomer"></param>
        /// <returns></returns>
        [HttpPost]
        public Response AddCustomer(ParamAddCustomer paramAddCustomer)
        {
            return customerApp.AddCustomer(paramAddCustomer);
        }
        [CustomerAuthorization]
        [HttpPost]
        public void Test()
        {
        }
    }
}