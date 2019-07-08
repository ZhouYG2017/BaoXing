using DoNet.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Data
{
    public class BasicRepository
    {
        string[] BeyondTableName = new string[] { "Sys_Log", "PriceGroupMappingGoods","Sys_UserLogOn", "Sys_RoleAuthorize", "BusinessDataOperateDetail" };
        /// <summary>
        /// copid
        /// </summary>
        public string companyId
        {
            get
            {
                OperatorModel operatorModel = OperatorProvider.Provider.GetCurrent();
                //获取不到当前登录信息，由控制器拦截，这里不处理
                if (!operatorModel.IsEmpty())
                {
                    return operatorModel.CompanyId;
                }
                return null;
            }
        }
        /// <summary>
        /// set copid
        /// </summary>
        /// <param name="entity"></param>
        protected TEntity SetCopId<TEntity>(TEntity entity)
        {
            Type type = entity.GetType();
            if (IsExistsCopId(type))
            {
                PropertyInfo[] propertyInfos = type.GetProperties();
                foreach (PropertyInfo info in propertyInfos)
                {
                    //存在租户ID字段且该字段无值，赋予当前登录人租户ID
                    if (info.Name == tenantColumnStr && info.GetValue(entity).IsEmpty())
                    {
                        info.SetValue(entity, companyId);
                    }
                }
            }
            return entity;
        }
        protected const string tenantColumnStr = "CopId";
        //protected DoNetDbContext dbcontext = new DoNetDbContext(); 
        protected DoNetDbContext dbcontext = DoNetDbContextFactory.GetSingleDbContext();
        protected Boolean RemoveHoldingEntityInContext<TEntity>(TEntity entity) where TEntity : class
        {
            var objContext = ((IObjectContextAdapter)dbcontext).ObjectContext;
            var objSet = objContext.CreateObjectSet<TEntity>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);

            if (exists)
            {
                objContext.Detach(foundEntity);
            }

            return (exists);
        }
        protected void WriteDataOperateLog<TEntity>(TEntity entity, DataOperateType dataOperateType) where TEntity : class
        {
            WriteDataOperateLog(new List<TEntity>() { entity }, dataOperateType);
        }
        protected void WriteDataOperateLog<TEntity>(List<TEntity> entities, DataOperateType dataOperateType) where TEntity:class
        {
            OperatorModel operatorModel = OperatorProvider.Provider.GetCurrent();
            if (entities != null && entities.Count > 0 && operatorModel != null)
            {
                string tableName = GetTableNameOrColumnName(typeof(TEntity), AcquiredEdmType.TableName);
                if(BeyondTableName.FirstOrDefault(n=>n == tableName) != null)
                {
                    return;
                }
                string primayKey = GetTableNameOrColumnName(typeof(TEntity), AcquiredEdmType.FirstPrimaryKeyNameString);
                PropertyInfo keyPropertyInfo = entities[0].GetType().GetProperties().FirstOrDefault(p => p.Name == primayKey);
                var type = typeof(TEntity);
                var attribute = type.GetCustomAttributes(typeof(DisplayNameAttribute), false).FirstOrDefault();
                foreach (var entity in entities)
                {
                    string keyValue = keyPropertyInfo.GetValue(entity).ToString();
                    TEntity oldEntity = dbcontext.Set<TEntity>().Find(keyValue);
                    //if (entity.IsEmpty() == false && CompareCopId(entity))
                    //{
                    //    oldEntity = null;
                    //}

                    DataOperateLogEntity dataOperateLogEntity = new DataOperateLogEntity()
                    {
                        F_Id = Guid.NewGuid().ToString(),
                        F_OperateType = dataOperateType.ToString(),
                        F_DataId = keyValue,
                        F_Account = operatorModel.UserCode,
                        F_NickName = operatorModel.UserName,
                        F_IPAddress = operatorModel.LoginIPAddress,
                        F_IPAddressName = operatorModel.LoginIPAddressName,
                        AssemblyName = entity.GetType().FullName,
                        ModelName = attribute == null ? "" : ((DisplayNameAttribute)attribute).DisplayName,
                        TableName = tableName,
                        OldData = oldEntity == null ? "" : oldEntity.ToJson(),
                        NewData = dataOperateType == DataOperateType.Delete ? "" : entity == null ? "" : entity.ToJson(),
                        F_CreatorTime = DateTime.Now,
                        F_CreatorUserId = operatorModel.UserId
                    };
                    dbcontext.Entry<DataOperateLogEntity>(dataOperateLogEntity).State = EntityState.Added;
                }
                dbcontext.SaveChanges();
            }
        }
        public string GetTableNameOrColumnName(Type type, AcquiredEdmType edmType, string propertyName = null)
        {
            if (dbcontext == null)
            {
                throw new ArgumentNullException("dbContext");
            }
            //if (entity == null)
            //{
            //    throw new ArgumentNullException("TEntity");
            //}
            //Type type = typeof(TEntity);
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            CachedEdmInfo edmInfo;
            if (MappingCache._mappingCache.ContainsKey(type.FullName))
            {
                edmInfo = MappingCache._mappingCache[type.FullName];
            }
            else
            {
                MetadataWorkspace metadata = ((IObjectContextAdapter)dbcontext).ObjectContext.MetadataWorkspace;

                // Get the part of the model that contains info about the actual CLR types
                ObjectItemCollection objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

                // Get the entity type from the model that maps to the CLR type
                EntityType entityType = metadata
                        .GetItems<EntityType>(DataSpace.OSpace)
                              .Single(e => objectItemCollection.GetClrType(e) == type);

                // Get the entity set that uses this entity type
                EntitySet entitySet = metadata
                    .GetItems<EntityContainer>(DataSpace.CSpace)
                          .Single()
                          .EntitySets
                          .Single(s => s.ElementType.Name == entityType.Name);

                // Find the mapping between conceptual and storage model for this entity set
                EntitySetMapping mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                              .Single()
                              .EntitySetMappings
                              .Single(s => s.EntitySet == entitySet);

                // Find the storage entity set (table) that the entity is mapped
                EntitySet tableEntitySet = mapping
                    .EntityTypeMappings.Single()
                    .Fragments.Single()
                    .StoreEntitySet;

                edmInfo = new CachedEdmInfo
                {
                    Mapping = mapping,
                    TableEntitySet = tableEntitySet
                };
            }


            // Return the table name from the storage entity set
            object objTableName = edmInfo.TableEntitySet.MetadataProperties["Table"].Value;
            string tableName = objTableName == null ? edmInfo.TableEntitySet.Name : Convert.ToString(objTableName);

            switch (edmType)
            {
                case AcquiredEdmType.TableName:
                    return tableName;
                case AcquiredEdmType.FirstPrimaryKeyNameString:
                    {
                        var firstKeyProp = edmInfo.TableEntitySet.ElementType.KeyProperties[0];
                        //return tableName + "." + firstKeyProp.Name;
                        return firstKeyProp.Name;
                    }
                case AcquiredEdmType.ColumnName:
                    {
                        // Find the storage property (column) that the property is mapped
                        edmInfo.PropertyColumnNameDic = edmInfo.PropertyColumnNameDic ?? new Dictionary<string, string>();
                        if (edmInfo.PropertyColumnNameDic.ContainsKey(propertyName))
                        {
                            return edmInfo.PropertyColumnNameDic[propertyName];
                        }
                        else
                        {
                            string columnName = edmInfo.Mapping
                            .EntityTypeMappings.Single()
                            .Fragments.Single()
                            .PropertyMappings
                            .OfType<ScalarPropertyMapping>()
                                  .Single(m => m.Property.Name == propertyName)
                            .Column
                            .Name;
                            //写入缓存
                            edmInfo.PropertyColumnNameDic.Add(propertyName, columnName);
                            //return tableName + "." + columnName;
                            return columnName;
                        }
                    }
                default:
                    throw new ArgumentNullException("Invalid argument");
            }
        }
        protected bool CompareCopId<TEntity>(TEntity entity)
        {
            Type type = entity.GetType();
            string copId = companyId;
            if (copId.IsEmpty() || IsExistsCopId(type) == false)
            {
                return true;
            }
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                //存在租户ID字段且该字段无值，赋予当前登录人租户ID
                if (info.Name == tenantColumnStr && info.GetValue(entity).IsEmpty() == false)
                {
                    if (info.GetValue(entity).ToString() != companyId)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// is exists copid
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool IsExistsCopId(Type type)
        {
            bool result = false;
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                //存在租户ID字段且该字段无值，赋予当前登录人租户ID
                if (info.Name == tenantColumnStr)
                {
                    result = true;
                }
            }
            return result;
        }
    }

    public static class MappingCache
    {
        public static Dictionary<string, CachedEdmInfo> _mappingCache = new Dictionary<string, CachedEdmInfo>();
    }
    public struct CachedEdmInfo
    {
        public EntitySetMapping Mapping { get; set; }

        public EntitySet TableEntitySet { get; set; }

        public Dictionary<string, string> PropertyColumnNameDic { get; set; }
    }
    public enum AcquiredEdmType
    {
        TableName,
        FirstPrimaryKeyNameString,
        ColumnName
    }
}
