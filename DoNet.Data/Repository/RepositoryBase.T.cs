/*******************************************************************************
 * Copyright © 2016 DoNet.Framework 版权所有
 * Author: DoNet
 * Description: DoNet快速开发平台
 * Website：http://www.DoNet.cn
*********************************************************************************/
using DoNet.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DoNet.Data
{
    /// <summary>
    /// 仓储实现
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class RepositoryBase<TEntity> : BasicRepository, IRepositoryBase<TEntity> where TEntity : class, new()
    {
        public int Insert(TEntity entity)
        {
            entity = SetCopId(entity);
            //WriteDataOperateLog(entity, DataOperateType.Create);
            dbcontext.Entry<TEntity>(entity).State = EntityState.Added;
            return dbcontext.SaveChanges();
        }
        public int Insert(List<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                TEntity et = SetCopId(entity);
            }
            WriteDataOperateLog(entitys, DataOperateType.Create);

            foreach (var entity in entitys)
            {
                dbcontext.Entry<TEntity>(entity).State = EntityState.Added;
            }
            return dbcontext.SaveChanges();
        }
        public int Update(TEntity entity)
        {
            if (!CompareCopId(entity))
            {
                throw new Exception("只能修改本公司数据");
            }
            WriteDataOperateLog(entity, DataOperateType.Update);

            RemoveHoldingEntityInContext(entity);
            dbcontext.Set<TEntity>().Attach(entity);
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                //加上这句如果数据库不存在该字段会报错
                //dbcontext.Entry(entity).Property(prop.Name).IsModified = true;
                if (prop.GetValue(entity, null) != null)
                {
                    if (prop.GetValue(entity, null).ToString() == "&nbsp;")
                    {
                        dbcontext.Entry(entity).Property(prop.Name).CurrentValue = null;
                    }
                }
                //过滤不更新数据库的字段
                if (prop.CustomAttributes.Where(n => n.AttributeType.Name == "NotMappedAttribute").Count() == 0)
                {
                    dbcontext.Entry(entity).Property(prop.Name).IsModified = true;
                }
            }
            return dbcontext.SaveChanges();
        }
        public int Updates(TEntity entity)
        {
            //只能修改自己公司的数据
            if (!CompareCopId(entity))
            {
                throw new Exception("只能修改本公司数据");
            }
            WriteDataOperateLog(entity, DataOperateType.Update);
            if (dbcontext.Set<TEntity>().Local.Contains(entity))
            {
                dbcontext.Entry(entity).State = EntityState.Modified;
            }
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (!prop.IsDefined(typeof(NotMappedAttribute)))
                {
                    dbcontext.Entry(entity).Property(prop.Name).IsModified = true;
                }
                if (prop.GetValue(entity, null) != null)
                {
                    if (prop.GetValue(entity, null).ToString() == "&nbsp;")
                    {
                        dbcontext.Entry(entity).Property(prop.Name).CurrentValue = null;
                    }
                }
            }
            return dbcontext.SaveChanges();
        }
        public int Delete(TEntity entity)
        {
            //只能修改自己公司的数据
            if (!CompareCopId(entity))
            {
                throw new Exception("只能修改本公司数据");
            }
            WriteDataOperateLog(entity, DataOperateType.Delete);
            dbcontext.Set<TEntity>().Attach(entity);
            dbcontext.Entry<TEntity>(entity).State = EntityState.Deleted;
            return dbcontext.SaveChanges();
        }
        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entitys = dbcontext.Set<TEntity>().Where(predicate).ToList();
            foreach (TEntity entity in entitys)
            {
                //只能修改自己公司的数据
                if (!CompareCopId(entity))
                {
                    throw new Exception("只能修改本公司数据");
                }
            }
            WriteDataOperateLog(entitys, DataOperateType.Delete);

            foreach (TEntity entity in entitys)
            {
                dbcontext.Entry<TEntity>(entity).State = EntityState.Deleted;
            }
            return dbcontext.SaveChanges();
        }
        public TEntity FindEntity(object keyValue)
        {
            TEntity entity = dbcontext.Set<TEntity>().Find(keyValue);
            if (entity.IsEmpty() == false && CompareCopId(entity))
            {
                return entity;
            }
            return null;
        }
        public bool IsExists(string keyValue)
        {
            if (FindEntity(keyValue) == null)
            {
                return false;
            }
            return true;
        }
        public TEntity FindEntity(Expression<Func<TEntity, bool>> predicate)
        {
            if (IsExistsCopId(typeof(TEntity)) && companyId.IsEmpty() == false)
            {
                Expression<Func<TEntity, bool>> expressionAdd = ExpressionHelper.CreateEqual<TEntity>(tenantColumnStr, companyId);
                predicate.AndExpression(expressionAdd);
            }
            
            return dbcontext.Set<TEntity>().FirstOrDefault(predicate);
        }
        public IQueryable<TEntity> IQueryable(bool IsTenantFillter = true)
        {
            if (IsTenantFillter && IsExistsCopId(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> expressionAdd = ExpressionHelper.CreateEqual<TEntity>(tenantColumnStr, companyId);
                return dbcontext.Set<TEntity>().Where(expressionAdd);
            }
            else
            {
                return dbcontext.Set<TEntity>();
            }
        }
        public IQueryable<TEntity> IQueryable(Expression<Func<TEntity, bool>> predicate, bool IsTenantFillter = true)
        {
            if (IsTenantFillter && IsExistsCopId(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> expressionAdd = ExpressionHelper.CreateEqual<TEntity>(tenantColumnStr, companyId);
                predicate.AndExpression(expressionAdd);
            }
            return dbcontext.Set<TEntity>().Where(predicate);
        }
        public List<TEntity> FindList(string strSql)
        {
            return dbcontext.Database.SqlQuery<TEntity>(strSql).ToList<TEntity>();
        }
        public List<TEntity> FindList(string strSql, DbParameter[] dbParameter)
        {
            return dbcontext.Database.SqlQuery<TEntity>(strSql, dbParameter).ToList<TEntity>();
        }
        public List<TEntity> FindList(Pagination pagination, string strSql, string SqlCondition = "")
        {
            var paginationSql = strSql + " order by " + pagination.sidx + " offset " + (pagination.page - 1) * pagination.rows + " rows fetch next " + pagination.rows + " rows only";
            var SqlCount = strSql;
            if (!string.IsNullOrEmpty(SqlCondition))
            {
                SqlCount = "select count(1) " + SqlCondition;
                pagination.records = dbcontext.Database.SqlQuery<int>(SqlCount).ToList()[0];
            }
            else
            {
                pagination.records = dbcontext.Database.SqlQuery<TEntity>(SqlCount).Count();
            }

            return dbcontext.Database.SqlQuery<TEntity>(paginationSql).ToList<TEntity>();
        }
        public List<TEntity> FindListBySQL2008(Pagination pagination, string strSql)
        {
            string sql = @"Select * from(select ROW_NUMBER() over(order by " + pagination.sidx + ") as __tempId," +
                            "    * From (" + strSql + ") as a) as a Where a.__tempId between " + (pagination.page - 1) * pagination.rows + " And " + pagination.page * pagination.rows;
            pagination.records = dbcontext.Database.SqlQuery<TEntity>(strSql).Count();
            return dbcontext.Database.SqlQuery<TEntity>(sql).ToList<TEntity>();
        }
        public List<TEntity> FindListBySQL2008(Pagination pagination, string strSql, SqlParamMgr spList)
        {
            if (spList == null) throw new Exception("FindListBySQL2008中spList参数为空");
            //string sql = @"Select * from(select ROW_NUMBER() over(order by " + pagination.sidx + ") as __tempId," +
            string sql = @"Select * from(select ROW_NUMBER() over(order by " + (typeof(TEntity).CheckPropertyInfo(pagination.sidx) ? "F_Id" : pagination.sidx) + ") as __tempId," +
                            "    * From (" + strSql + ") as a) as a Where a.__tempId between " + (pagination.page - 1) * pagination.rows + " And " + pagination.page * pagination.rows;

            List<System.Data.SqlClient.SqlParameter> list = new List<System.Data.SqlClient.SqlParameter>();
            if (spList.ParamList.Count > 0)
            {
                //复制一份参数            
                foreach (SqlParameter p in spList.ParamList)
                {
                    SqlParameter pp = (SqlParameter)((ICloneable)p).Clone();
                    list.Add(pp);
                }
            }
            //list.Add(new System.Data.SqlClient.SqlParameter("@pagination_sidx", pagination.sidx));

            //总数
            string sqlCount = "select count(*) from (" + strSql + ") t_asdfgh12";
            DbRawSqlQuery<int> reRecords = dbcontext.Database.SqlQuery<int>(sqlCount, spList.ToArray());
            pagination.records = reRecords.FirstOrDefault();
            //记录,需要排序
            return dbcontext.Database.SqlQuery<TEntity>(sql, list.ToArray()).ToList<TEntity>();
        }
        public List<TEntity> FindListBySQL2008(string strSql, SqlParamMgr spList)
        {
            if (spList == null) throw new Exception("FindListBySQL2008中spList参数为空");

            List<System.Data.SqlClient.SqlParameter> list = new List<System.Data.SqlClient.SqlParameter>();
            if (spList.ParamList.Count > 0)
            {
                //复制一份参数            
                foreach (SqlParameter p in spList.ParamList)
                {
                    SqlParameter pp = (SqlParameter)((ICloneable)p).Clone();
                    list.Add(pp);
                }
            }
            //记录,需要排序
            return dbcontext.Database.SqlQuery<TEntity>(strSql, list.ToArray()).ToList<TEntity>();
        }

        //public List<T> FindListBySQL2008<T>(string strSql, SqlParamMgr spList)
        //{
        //    if (spList == null) throw new Exception("FindListBySQL2008中spList参数为空");

        //    List<System.Data.SqlClient.SqlParameter> list = new List<System.Data.SqlClient.SqlParameter>();
        //    if (spList.ParamList.Count > 0)
        //    {
        //        //复制一份参数            
        //        foreach (SqlParameter p in spList.ParamList)
        //        {
        //            SqlParameter pp = (SqlParameter)((ICloneable)p).Clone();
        //            list.Add(pp);
        //        }
        //    }
        //    //记录,需要排序
        //    return dbcontext.Database.SqlQuery<T>(strSql, list.ToArray()).ToList<T>();
        //}
        public TEntity FindEntiryBySQL2008(string strSql, SqlParamMgr spList)
        {
            if (spList == null) throw new Exception("FindListBySQL2008中spList参数为空");

            List<System.Data.SqlClient.SqlParameter> list = new List<System.Data.SqlClient.SqlParameter>();
            if (spList.ParamList.Count > 0)
            {
                //复制一份参数            
                foreach (SqlParameter p in spList.ParamList)
                {
                    SqlParameter pp = (SqlParameter)((ICloneable)p).Clone();
                    list.Add(pp);
                }
            }
            //记录,需要排序
            return dbcontext.Database.SqlQuery<TEntity>(strSql, list.ToArray()).FirstOrDefault<TEntity>();
        }

        public T FindEntiryBySQL2008<T>(string strSql, SqlParamMgr spList)
        {
            if (spList == null) throw new Exception("FindListBySQL2008中spList参数为空");

            List<System.Data.SqlClient.SqlParameter> list = new List<System.Data.SqlClient.SqlParameter>();
            if (spList.ParamList.Count > 0)
            {
                //复制一份参数            
                foreach (SqlParameter p in spList.ParamList)
                {
                    SqlParameter pp = (SqlParameter)((ICloneable)p).Clone();
                    list.Add(pp);
                }
            }
            //记录,需要排序
            return dbcontext.Database.SqlQuery<T>(strSql, list.ToArray()).FirstOrDefault<T>();
        }
        protected int ExecuteSqlCommand(string strSql, SqlParamMgr spList)
        {
            if (spList == null) throw new Exception("FindListBySQL2008中spList参数为空");

            List<System.Data.SqlClient.SqlParameter> list = new List<System.Data.SqlClient.SqlParameter>();
            if (spList.ParamList.Count > 0)
            {
                //复制一份参数            
                foreach (SqlParameter p in spList.ParamList)
                {
                    SqlParameter pp = (SqlParameter)((ICloneable)p).Clone();
                    list.Add(pp);
                }
            }
            //记录,需要排序
            return dbcontext.Database.ExecuteSqlCommand(strSql, list.ToArray());
        }
        public int UpdateEntity(string strSql, SqlParamMgr spList)
        {
            if (spList == null)
            {
                throw new Exception("UpdateEntity中spList参数为空");
            }

            List<System.Data.SqlClient.SqlParameter> list = new List<System.Data.SqlClient.SqlParameter>();
            if (spList.ParamList.Count > 0)
            {
                //复制一份参数            
                foreach (SqlParameter p in spList.ParamList)
                {
                    SqlParameter pp = (SqlParameter)((ICloneable)p).Clone();
                    list.Add(pp);
                }
            }

            return dbcontext.Database.ExecuteSqlCommand(strSql, spList.ToArray());
        }
        public List<TEntity> FindList(Pagination pagination)
        {
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = dbcontext.Set<TEntity>().AsQueryable();
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(TEntity), "t");
                var property = typeof(TEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
            return tempData.ToList();
        }
        public List<TEntity> FindList(Expression<Func<TEntity, bool>> predicate, Pagination pagination)
        {
            bool isAsc = pagination.sord.ToLower() == "asc" ? true : false;
            string[] _order = pagination.sidx.Split(',');
            MethodCallExpression resultExp = null;
            var tempData = dbcontext.Set<TEntity>().Where(predicate);
            foreach (string item in _order)
            {
                string _orderPart = item;
                _orderPart = Regex.Replace(_orderPart, @"\s+", " ");
                string[] _orderArry = _orderPart.Split(' ');
                string _orderField = _orderArry[0];
                bool sort = isAsc;
                if (_orderArry.Length == 2)
                {
                    isAsc = _orderArry[1].ToUpper() == "ASC" ? true : false;
                }
                var parameter = Expression.Parameter(typeof(TEntity), "t");
                var property = typeof(TEntity).GetProperty(_orderField);
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                resultExp = Expression.Call(typeof(Queryable), isAsc ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, tempData.Expression, Expression.Quote(orderByExp));
            }
            tempData = tempData.Provider.CreateQuery<TEntity>(resultExp);
            pagination.records = tempData.Count();
            tempData = tempData.Skip<TEntity>(pagination.rows * (pagination.page - 1)).Take<TEntity>(pagination.rows).AsQueryable();
            return tempData.ToList();
        }
    }
}