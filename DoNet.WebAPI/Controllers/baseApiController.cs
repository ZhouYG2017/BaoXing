using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DoNet.Code;

namespace DoNet.WebAPI.Controllers
{
    public class baseApiController : ApiController
    {
        /// <summary>
        /// API返回信息
        /// </summary>
        /// <param name="success"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public HttpResponseMessage ToResponseMessage(bool success, string code, string message)
        {
            HttpStatusCode httpStatusCode = success ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
            return new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(new
                {
                    Code = code,
                    Message = message
                }.ToJson())
            };
        }
    }
}
