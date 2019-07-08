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
    public class DataListBase<T> : RepositoryBase<T> where T : class, new()
    {
        /// <summary>
        /// 列表分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pagination"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        public List<T> GetPageList(Pagination pagination, List<ParamDataFilter> datas)
        {

            IQueryable<T> source = dbcontext.Set<T>();
            if (datas != null && datas.Count > 0)
            {
                source = DataFilter(source, datas);
            }
            if (IsExistsCopId(typeof(T)))
            {
                Expression<Func<T, bool>> copIdExp = ExpressionHelper.GetContains<T>("CopId", OperatorProvider.Provider.GetCurrent().CompanyId);
                source = source.Where(copIdExp);
            }
            if (pagination != null)
            {
                string sortExpression = pagination.sidx;
                string sortDirection = pagination.sord;
                //错误查询
                if (!string.IsNullOrEmpty(sortExpression))// || string.IsNullOrEmpty(sortDirection))
                {
                    string sortingDir = string.Empty;
                    if (sortDirection.ToUpper().Trim() == "ASC")
                    {
                        sortingDir = "OrderBy";
                    }
                    //默认按降序排序
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
        private IQueryable<T> DataFilter(IQueryable<T> source, IEnumerable<ParamDataFilter> datas)
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
                                //expression = ExpressionHelper.And(expression, ExpressionOperate(Expression.Equal, p, item.FilterValue));
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateEqual<T>(p.Name, item.FilterValue, propertype));
                                break;
                            }
                        case ContrastType.NotEqual:
                            {
                                /* 不等于 */
                                //expression = ExpressionHelper.And(expression, ExpressionOperate(Expression.NotEqual, p, item.FilterValue));
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateNotEqual<T>(p.Name, item.FilterValue, propertype));
                                break;
                            }

                        case ContrastType.Greater:
                            {
                                /* 大于 */
                                //expression = ExpressionHelper.And(expression, ExpressionOperate(Expression.GreaterThan, p, item.FilterValue));
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateGreaterThan<T>(p.Name, item.FilterValue, propertype));
                                break;
                            }

                        case ContrastType.GreaterorEqual:
                            {
                                /* 大于等于 */
                                //expression = ExpressionHelper.And(expression, ExpressionOperate(Expression.GreaterThanOrEqual, p, item.FilterValue));
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateGreaterThanOrEqual<T>(p.Name, item.FilterValue, propertype));
                                break;
                            }
                        case ContrastType.Less:
                            {
                                /* 小于 */
                                //expression = ExpressionHelper.And(expression, ExpressionOperate(Expression.LessThan, p, item.FilterValue));
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateLessThan<T>(p.Name, item.FilterValue, propertype));
                                break;
                            }
                        case ContrastType.LessorEqual:
                            {
                                /* 小于等于 */
                                //expression = ExpressionHelper.And(expression, ExpressionOperate(Expression.LessThanOrEqual, p, item.FilterValue));
                                Type propertype = p.PropertyType;
                                expression = expression.And(ExpressionHelper.CreateLessThanOrEqual<T>(p.Name, item.FilterValue, propertype));
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


        ///<summary>
        ///拼接Lambda表达式过滤数据
        ///</summary>
        ///<typeparam name="T"></typeparam>
        //<typeparam name="V"></typeparam>
        ///<paramname="operateExpression"></ param>
        ///<param name="source"></param>
        ///<param name="p"></param> ///<param name="value"></param>
        ///<returns></returns>
        protected Expression<Func<T, bool>> ExpressionOperate<V>(ExpressionOpretaDelegate operateExpression, PropertyInfo p, V value)
        {
            Expression right = null;
            if (p.PropertyType == typeof(Int32))
            {
                int val = Convert.ToInt32(value);
                right = Expression.Constant(val, p.PropertyType);
            }
            else if (p.PropertyType == typeof(Decimal))
            {
                Decimal val = Convert.ToDecimal(value);
                right = Expression.Constant(val, p.PropertyType);
            }
            else if (p.PropertyType == typeof(Byte))
            {
                Byte val = Convert.ToByte(value);
                right = Expression.Constant(val, p.PropertyType);
            }
            else
            {
                right = Expression.Constant(value, p.PropertyType);
            }
            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            Expression left = Expression.Property(param, p.Name);
            Expression filter = operateExpression(left, right);
            return Expression.Lambda<Func<T, bool>>(filter, param);
        }

        ///<summary>
        ///字符串包含操作
        ///</summary>
        ///<param name="left"></param>
        ///<param name="right"></param>
        ///<returns></returns>
        private Expression StringContains(Expression left, Expression right)
        {
            Expression filter = Expression.Call(left, typeof(string).GetMethod("Contains"), right);
            return filter;
        }
    }

}
