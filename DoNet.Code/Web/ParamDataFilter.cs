using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Code
{
    /// <summary>
    /// 搜索参数
    /// </summary>
    public class ParamDataFilter
    {
        /// <summary>
        /// 对应字段
        /// </summary>
        public string FilterField { get; set; }
        /// <summary>
        /// 匹配值
        /// </summary>
        public string FilterValue { get; set; }
        /// <summary>
        /// 匹配类型
        /// </summary>
        public string FilterOp { get; set; }
        /// <summary>
        /// 1时间 2like 3 Not In/In  其他
        /// </summary>
        public string DataType { get; set; }
    }
}
