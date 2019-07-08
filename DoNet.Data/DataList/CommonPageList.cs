using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DoNet.Code;

namespace DoNet.Data
{
    public class CommonPageList : ICommonPageList
    {
        private DoNetDbContext dbcontext = DoNetDbContextFactory.GetSingleDbContext();

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public List<T> GetPageList<T>(Pagination pagination, List<ParamDataFilter> datas) where T : class, new()
        {
            IQueryable<T> source = dbcontext.Set<T>();
            if (datas != null && datas.Count > 0)
            {
                source = DataFilter(source, datas);
            }
            if (typeof(T).CheckPropertyInfo("CopId"))
            {
                Expression<Func<T, bool>> copIdExp = ExpressionHelper.CreateEqual<T>("CopId", OperatorProvider.Provider.GetCurrent().CompanyId);
                source = source.Where(copIdExp);
            }
            if (pagination != null)
            {
                string sortExpression = pagination.sidx;
                string sortDirection = pagination.sord;
                if (sortExpression.IsEmpty()==false&&typeof(T).CheckPropertyInfo(sortExpression))
                {
                    string sortingDir = string.Empty;
                    if (sortDirection.ToUpper().Trim() == "ASC")
                    {
                        sortingDir = "OrderBy";
                    }
                    else if (sortDirection.ToUpper().Trim() == "DESC" || sortDirection.IsEmpty())
                    {
                        sortingDir = "OrderByDescending";
                    }
                    ParameterExpression param = Expression.Parameter(typeof(T), sortExpression);
                    PropertyInfo pi = typeof(T).GetProperty(sortExpression);
                    Type[] types = new Type[2];
                    types[0] = typeof(T);
                    types[1] = pi.PropertyType;
                    Expression expr = Expression.Call(typeof(Queryable), sortingDir, types, source.Expression, Expression.Lambda(Expression.Property(param, sortExpression), param));
                    source = source.AsQueryable().Provider.CreateQuery<T>(expr);
                }
                pagination.records = source.Count();
                if (pagination.page <= 1)
                {
                    source = source.Take(pagination.rows);
                }
                else
                {
                    source = source.Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                }
            }
            return source.ToList();
        }

        /// <summary>
        /// 列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public List<T> GetPageList<T>(Pagination pagination, List<ParamDataFilter> datas, Expression<Func<T, bool>> extExpress) where T : class, new()
        {
            IQueryable<T> source = dbcontext.Set<T>();
            if (datas != null && datas.Count > 0)
            {
                source = DataFilter(source, datas);
            }
            if (typeof(T).CheckPropertyInfo("CopId"))
            {
                Expression<Func<T, bool>> copIdExp = ExpressionHelper.GetContains<T>("CopId", OperatorProvider.Provider.GetCurrent().CompanyId);
                source = source.Where(copIdExp);
            }
            if (extExpress != null)
            {
                source = source.Where(extExpress);
            }
            if (pagination != null)
            {
                string sortExpression = pagination.sidx;
                if (!sortExpression.IsEmpty() && typeof(T).CheckPropertyInfo(sortExpression))
                {
                    string sortDirection = pagination.sord;
                    if (!string.IsNullOrEmpty(sortExpression))
                    {
                        string sortingDir = string.Empty;
                        if (sortDirection.ToUpper().Trim() == "ASC")
                        {
                            sortingDir = "OrderBy";
                        }
                        else if (sortDirection.ToUpper().Trim() == "DESC" || sortDirection.IsEmpty())
                        {
                            sortingDir = "OrderByDescending";
                        }
                        ParameterExpression param = Expression.Parameter(typeof(T), sortExpression);
                        PropertyInfo pi = typeof(T).GetProperty(sortExpression);
                        Type[] types = new Type[2];
                        types[0] = typeof(T);
                        types[1] = pi.PropertyType;
                        Expression expr = Expression.Call(typeof(Queryable), sortingDir, types, source.Expression, Expression.Lambda(Expression.Property(param, sortExpression), param));
                        source = source.AsQueryable().Provider.CreateQuery<T>(expr);
                    }
                    pagination.records = source.Count();
                    if (pagination.page <= 1)
                    {
                        source = source.Take(pagination.rows);
                    }
                    else
                    {
                        source = source.Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                    }
                }
            }
            return source.ToList();
        }



        ///<summary>
        ///表达式操作
        ///</summary>
        ///<param name="right"></param>
        ///<param name="left"></param>
        ///<returns></returns>
        public delegate Expression ExpressionOpretaDelegate(Expression left, Expression right);

        ///<summary>///数据列表过滤
        ///</summary>
        ///<typeparam name="T">过滤的数据类型</typeparam>
        ///<param name="source">过滤的数据源</param>
        ///<paramname="dataFilterList">过滤条件集合(包含,字段名,值,操作符) </param>
        ///<returns></returns>
        private IQueryable<T> DataFilter<T>(IQueryable<T> source, IEnumerable<ParamDataFilter> datas)
        {
            Expression<Func<T, bool>> expression = ExpressionHelper.True<T>();
            T obj = System.Activator.CreateInstance<T>();
            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (var item in datas)
            {
                PropertyInfo p = properties.Where(pro => pro.Name == item.FilterField).FirstOrDefault();
                //不进行无效过滤
                if (p == null || item.FilterField.IsEmpty() || item.FilterValue.IsEmpty() || item.FilterOp.IsEmpty())
                {
                    continue;
                }
                else
                {
                    if (ContrastType.CheckContrastType(item.FilterOp) == false)
                    {
                        continue;
                    }
                    switch (item.FilterOp)
                    {
                        case ContrastType.In:
                            {
                                //List<string> filterValueList = item.FilterValue.Split(',').ToList();
                                //var newlist = filterValueList.Select<string, int>(x => Convert.ToInt32(x)).ToList();
                                ///* 包含, 目前只支持字符串 */
                                //expression = expression.And(ExpressionHelper.GetContains<T>(p.Name, newlist));
                                //break;

                                Type propertype = p.PropertyType;

                                List<string> filterValueList = item.FilterValue.Split(',').ToList();
                                //var newlist = filterValueList.Select<string, int>(x => Convert.ToInt32(x)).ToList();
                                Expression<Func<T, bool>> s = null;
                                foreach (string objfor in filterValueList)
                                {
                                    if (s == null)
                                    {
                                        s = ExpressionHelper.CreateEqual<T>(p.Name, objfor, propertype);
                                    }
                                    else
                                    {
                                        s = s.Or(ExpressionHelper.CreateEqual<T>(p.Name, objfor, propertype));
                                    }
                                }
                                /* 包含, 目前只支持字符串 */
                                expression = expression.And(s);
                                break;
                            }
                        case ContrastType.Contains:
                            {
                                /* 包含, 目前只支持字符串 */
                                expression = expression.And(ExpressionHelper.GetContains<T>(p.Name, item.FilterValue));
                                break;
                            }
                        case ContrastType.NotContains:
                            {
                                /* 包含, 目前只支持字符串 */
                                expression = expression.And(ExpressionHelper.GetNotContains<T>(p.Name, item.FilterValue));
                                break;
                            }
                        case ContrastType.Equal:
                            {
                                /* 等于 */
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateEqual<T>(p.Name, item.FilterValue, propertype));
                                break;
                            }
                        case ContrastType.NotEqual:
                            {
                                /* 不等于 */
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateNotEqual<T>(p.Name, item.FilterValue, propertype));
                                break;
                            }

                        case ContrastType.Greater:
                            {
                                /* 大于 */
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateGreaterThan<T>(p.Name, item.FilterValue, propertype));
                                break;
                            }

                        case ContrastType.GreaterorEqual:
                            {
                                /* 大于等于 */
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateGreaterThanOrEqual<T>(p.Name, item.FilterValue, propertype));
                                break;
                            }
                        case ContrastType.Less:
                            {
                                /* 小于 */
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateLessThan<T>(p.Name, item.FilterValue, propertype));
                                break;
                            }
                        case ContrastType.LessorEqual:
                            {
                                /* 小于等于 */

                                Type propertype = p.PropertyType;
                                string filterValue = item.FilterValue;
                                if (propertype.GenericTypeArguments.Contains(typeof(DateTime)))
                                {
                                    DateTime? endDate = filterValue.ToEndDate();
                                    if (endDate != null)
                                    {
                                        filterValue = endDate.ToString();
                                    }
                                }

                                expression = expression.And(ExpressionHelper.CreateLessThanOrEqual<T>(p.Name, filterValue, propertype));
                                break;
                            }
                        default: break;
                    }
                }
            }
            if (expression != null)
            {
                source = source.Where(expression);
            }
            return source;
        }
    }
}
