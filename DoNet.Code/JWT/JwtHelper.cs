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

        //私钥  web.config中配置
        //"GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
        private static string secret = ConfigurationManager.AppSettings["Secret"].ToString();

        /// <summary>
        /// 生成JwtToken
        /// </summary>
        /// <param name="payload">不敏感的用户数据</param>
        /// <returns></returns>
        public static string SetJwtEncode(Dictionary<string, object> payload)
        {
            IDateTimeProvider provider = new UtcDateTimeProvider();
            var now = provider.GetNow();

            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // or use JwtValidator.UnixEpoch
            var secondsSinceEpoch = Math.Round((now - unixEpoch).TotalSeconds);

            Console.WriteLine(secondsSinceEpoch);

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(payload, secret);
            return token;

        }

        /// <summary>
        /// 根据jwtToken获取实体
        /// </summary>
        /// <param name="token">jwtToken</param>
        /// <returns></returns>
        public static T GetJwtDecode<T>(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var json = decoder.Decode(token, secret, verify: true);//token为之前生成的字符串

                return json.ToObject<T>();
            }
            catch (TokenExpiredException)
            {
                throw new Exception("Token过期");
            }
            catch (SignatureVerificationException)
            {
                throw new Exception("Token无效");
            }
            catch (Exception)
            {
                throw new Exception("Token无效");
            }
        }
    }
}
