using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Code
{
    public static class ConvertToObejct
    {
        public static List<T> ToList<T>(this DataTable dt)
        {
            var dataColumn = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

            var properties = typeof(T).GetProperties();
            string columnName = string.Empty;
            return dt.AsEnumerable().Select(row =>
            {
                var t = Activator.CreateInstance<T>();
                foreach (var p in properties)
                {
                    columnName = p.Name;
                    if (dataColumn.Contains(columnName))
                    {
                        if (!p.CanWrite)
                            continue;

                        object value = row[columnName];
                        Type type = p.PropertyType;
                        if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))//判断convertsionType是否为nullable泛型类  
                        {
                            //如果type为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换  
                            System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(type);
                            //将type转换为nullable对的基础基元类型  
                            type = nullableConverter.UnderlyingType;
                        }
                        if (value != DBNull.Value)
                        {
                            p.SetValue(t, Convert.ChangeType(value, type), null);
                        }
                    }
                }
                return t;
            }).ToList();
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> varlist)
        //public static DataTable ToDataTable<T>(this IEnumerable<T> varlist, CreateRowDelegate<T> fn)
        {
            DataTable dtReturn = new DataTable();
            // column names
            PropertyInfo[] oProps = null;
            // Could add a check to verify that there is an element 0
            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType; if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow(); foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue(rec, null);
                }
                dtReturn.Rows.Add(dr);
            }
            return (dtReturn);
        }
        public delegate object[] CreateRowDelegate<T>(T t);
    }
}
