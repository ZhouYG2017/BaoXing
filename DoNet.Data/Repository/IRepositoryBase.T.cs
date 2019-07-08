/*******************************************************************************
 * Copyright © 2016 DoNet.Framework 版权所有
 * Author: DoNet
 * Description: DoNet快速开发平台
 * Website：http://www.DoNet.cn
*********************************************************************************/
using DoNet.Code;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;

namespace DoNet.Data
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IRepositoryBase<TEntity> where TEntity : class,new()
    {
        int Insert(TEntity entity);
        int Insert(List<TEntity> entitys);
        int Update(TEntity entity);
        int Updates(TEntity entity);
        int Delete(TEntity entity);
        int Delete(Expression<Func<TEntity, bool>> predicate);
        TEntity FindEntity(object keyValue);
        bool IsExists(string keyValue);
        TEntity FindEntity(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> IQueryable(bool IsTenantFillter = true);
        IQueryable<TEntity> IQueryable(Expression<Func<TEntity, bool>> predicate, bool IsTenantFillter = true);
        List<TEntity> FindList(string strSql);
        List<TEntity> FindList(string strSql, DbParameter[] dbParameter);

        List<TEntity> FindList(Pagination pagination, string strSql, string SqlCondition = "");
        List<TEntity> FindList(Pagination pagination);
        List<TEntity> FindList(Expression<Func<TEntity, bool>> predicate, Pagination pagination);
        TEntity FindEntiryBySQL2008(string strSql, SqlParamMgr spList);
        List<TEntity> FindListBySQL2008(Pagination pagination, string strSql);
        //List<T> FindListBySQL2008<T>(string strSql, SqlParamMgr spList);
        T FindEntiryBySQL2008<T>(string strSql, SqlParamMgr spList);
        List<TEntity> FindListBySQL2008(Pagination pagination, string strSql, SqlParamMgr spList);
        List<TEntity> FindListBySQL2008(string strSql, SqlParamMgr spList);
    }
}
