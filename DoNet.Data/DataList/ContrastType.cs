using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Data
{

    public class ContrastType
    {
        public const string Contains = "like";
        public const string NotContains = "no like";
        public const string Equal = "=";
        public const string NotEqual = "<>";
        public const string Greater = ">";
        public const string GreaterorEqual = ">=";
        public const string Less = "<";
        public const string LessorEqual = "<=";
        public const string In = "In";

        /// <summary>
        /// 验证操作符是否合法
        /// </summary>
        /// <param name="contrastTypeStr"></param>
        /// <returns></returns>
        public static bool CheckContrastType(string contrastTypeStr)
        {
            bool isExists = false;
            //ContrastType contrastType = new ContrastType();
            Type type = typeof(ContrastType);
            FieldInfo[] fieldInfos = type.GetFields();

            //fieldInfo.GetRawConstantValue().ToString()
            //var properties = typeof(ContrastType).GetProperties();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                var obj = fieldInfo.GetRawConstantValue();
                if (obj != null && obj.ToString().ToLower() == contrastTypeStr.ToLower())
                {
                    isExists = true;
                }
            }
            return isExists;
        }
    }
}
