using DoNet.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Data
{
    public interface ICommonPageList
    {
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        List<T> GetPageList<T>(Pagination pagination, List<ParamDataFilter> datas) where T : class, new();
        List<T> GetPageList<T>(Pagination pagination, List<ParamDataFilter> datas, Expression<Func<T, bool>> extExpress) where T : class, new();
    }
}
