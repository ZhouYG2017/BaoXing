using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Code
{
    public enum DataOperateType
    {
        [Description("新增")]
        Create = 1,
        [Description("修改")]
        Update = 2,
        [Description("删除")]
        Delete = 3
    }
}
