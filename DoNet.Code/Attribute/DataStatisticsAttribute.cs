using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Code
{

    /// <summary>
    /// 是否参与数据统计
    /// </summary>
    public class DataStatisticsAttribute : System.Attribute
    {
        private bool isStatistics;

        public DataStatisticsAttribute(bool IsStatistics = false)
        {
            this.isStatistics = IsStatistics;
        }
        public bool IsStatistics
        {
            get
            {
                return isStatistics;
            }
        }
    }
}
