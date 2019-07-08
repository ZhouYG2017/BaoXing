using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DoNet.Data
{
    public class DoNetDbContextFactory
    {
        public static DoNetDbContext GetUnitOfWorkDbContext()
        {
            //确保线程内上下文对象唯一
            DoNetDbContext dbContext = CallContext.GetData("DoNetDbContext") as DoNetDbContext;
            if (dbContext == null)
            {
                dbContext = new DoNetDbContext();
                CallContext.SetData("DoNetDbContext", dbContext);
            }
            return dbContext;
        }
        public static DoNetDbContext GetSingleDbContext()
        {
            //确保线程内上下文对象唯一
            DoNetDbContext dbContext = CallContext.GetData("DoNetDbContext") as DoNetDbContext;
            if (dbContext == null)
            {
                dbContext = new DoNetDbContext();
                CallContext.SetData("DoNetDbContext", dbContext);
            }
            return dbContext;
        }
        public static void RemoveUnitOfWorkDbContext()
        {
            CallContext.SetData("DoNetDbContext", null);
        }
    }
}
