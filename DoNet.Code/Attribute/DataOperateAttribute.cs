using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Code
{
    public class DataOperateAttribute : System.Attribute
    {
        private bool isWriteLog;

        public DataOperateAttribute(bool IsWriteLog = false)
        {
            this.isWriteLog = IsWriteLog;
        }
        public bool IsWriteLog
        {
            get
            {
                return isWriteLog;
            }
        }
    }

}
