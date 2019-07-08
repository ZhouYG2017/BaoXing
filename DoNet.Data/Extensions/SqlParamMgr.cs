using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Data
{
    /// <summary>
    /// sql参数管理
    /// </summary>
    public class SqlParamMgr
    {
        public SqlParamMgr()
        {
            ParamList = new List<System.Data.SqlClient.SqlParameter>();
        }
        public List<System.Data.SqlClient.SqlParameter> ParamList
        {
            get; set;
        }

        public void AddParem(string name, object val)
        {
            ParamList.Add(new SqlParameter(name, val));
        }
        public void Clear()
        {
            ParamList.Clear();
        }
        public SqlParameter[] ToArray()
        {
            if (ParamList != null)
            {
                return ParamList.ToArray();
            }
            else
            {
                return null;
            }
        }
    }
}
