using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Domain.ViewModel
{
    public class Response
    {
        public string  Code { get; set; }
        public string Message { get; set; }
    }

    public class Response<T> : Response
    {
        public T Data;
    }
}
