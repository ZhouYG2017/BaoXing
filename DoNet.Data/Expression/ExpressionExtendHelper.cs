using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Data
{
    public static class ExpressionExtendHelper
    {
        /////<summary>
        /////表达式操作
        /////</summary>
        /////<param name="right"></param>
        /////<param name="left"></param>
        /////<returns></returns>
        //public delegate Expression ExpressionOpretaDelegate(Expression left, Expression right);
        //public static Expression<T> ComposeExpression<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        //{
        //    var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
        //    var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
        //    return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        //}
        //public static Expression<Func<T, bool>> AndExpression<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        //{
        //    if(first == null)
        //    {
        //        return second;
        //    }
        //    return first.ComposeExpression(second, Expression.And);
        //}
        //public static Expression<Func<T, bool>> OrExpression<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        //{
        //    if (first == null)
        //    {
        //        return second;
        //    }
        //    return first.ComposeExpression(second, Expression.Or);
        //}

    }
}
