using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DoNet.Domain.ViewModel
{
    public class ParamLogin
    {
        [Required(ErrorMessage = "用户名不能为空")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage ="密码不能为空")]
        [StringLength(8, MinimumLength=4,ErrorMessage="密码长度为4-8位")]
        public string Password { get; set; }
    }
}
