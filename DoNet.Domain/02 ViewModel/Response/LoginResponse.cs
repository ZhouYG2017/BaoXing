using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Domain.ViewModel
{
    public class LoginResponse
    {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string Token { get; set; }
    }
}
