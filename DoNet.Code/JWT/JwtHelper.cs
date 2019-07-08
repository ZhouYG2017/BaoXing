using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Code
{
    public class JwtHelper
    {

        ////私钥  web.config中配置
        ////"GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        //private static string secret = ConfigurationManager.AppSettings["Secret"].ToString();

        ///// <summary>
        ///// 生成JwtToken
        ///// </summary>
        ///// <param name="payload">不敏感的用户数据</param>
        ///// <returns></returns>
        //public static string SetJwtEncode(Dictionary<string, object> payload)
        //{
        //    return token;
        //}

        ///// <summary>
        ///// 根据jwtToken  获取实体
        ///// </summary>
        ///// <param name="token">jwtToken</param>
        ///// <returns></returns>
        //public static UserInfo GetJwtDecode(string token)
        //{
        //}
    }
}
