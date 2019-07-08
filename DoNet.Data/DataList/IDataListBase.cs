using DoNet.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Data
{
    public interface IDataListBase<T> : IRepositoryBase<T> where T : class, new()
    {
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        List<T> GetPageList(Pagination pagination, List<ParamDataFilter> datas);
    }
}
