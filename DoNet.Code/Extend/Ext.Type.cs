using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoNet.Code
{
    public static class ExtType
    {
        public static bool CheckPropertyInfo(this Type type, string propertyName)
        {
            bool result = false;
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                //存在租户ID字段且该字段无值，赋予当前登录人租户ID
                if (info.Name.ToLower() == propertyName.ToLower())
                {
                    result = true;
                    if (info.IsDefined(typeof(NotMappedAttribute), true))
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
    }
}
