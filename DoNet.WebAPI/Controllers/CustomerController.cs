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
    public class CustomerController : baseApiController
    {
        private CustomerApp customerApp = new CustomerApp();

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="paramLogin"></param>
        /// <returns></returns>
        [HttpGet]
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
        public HttpResponseMessage AddCustomer(ParamAddCustomer paramAddCustomer)
        {
            var result = customerApp.AddCustomer(paramAddCustomer);
            if (result)
            {
                return ToResponseMessage(true, "200", "新增用户成功");
            }
            else
            {
                return ToResponseMessage(false, "500", "新增用户出错");
            }
        }
    }
}