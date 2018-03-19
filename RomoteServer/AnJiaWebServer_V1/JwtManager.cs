using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AnJiaWebServer_V1
{
    public  class JwtManager
    {


        /// <summary>
        /// 使用以下代码来生成对称密钥
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        /// 
        private static JwtManager uniqueInstance;//定义一个静态变量来保存类的实例
        private static readonly object locker = new object();//定义一个标识符确保线程同步

        private static JwtSecurityTokenHandler tokenHandler;

        private JwtManager()
        {
            tokenHandler = new JwtSecurityTokenHandler();
        }

        /// <summary>
        /// JwtManager全局访问点
        /// </summary>
        /// <returns></returns>
        public static JwtManager GetJwtManager()
        {
        
            if (uniqueInstance == null)    // 首先判断实例是不是为空
            {
                lock (locker)            // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new JwtManager();
                    }
                }
                // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
            }
            return uniqueInstance;
        }
        /// <summary>
        /// 获取Token
        /// </summary>
        /// <param name="username">登录用户名</param>
        /// <param name="expireMinutes">过期时间</param>
        /// <returns>Token String</returns>
        public  string GenerateToken(string username)
        {
            int expireMinutes = 720;
            var symmetricKey = Convert.FromBase64String(Constants.SecretKey);
            var now = DateTime.UtcNow;
            var s = now.AddMinutes(Convert.ToInt32(expireMinutes));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                    new Claim(ClaimTypes.Name,username),//
                    new Claim(JwtClaimTypes.Name,username),
                    new Claim(JwtClaimTypes.Role,"role"),
                    new Claim(JwtClaimTypes.Audience,Constants.Audience),
                    new Claim(JwtClaimTypes.Issuer,Constants.Issuer),

                        }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature),

              
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public string GenerateShareCode(string username, string DeviceID)//生成分享码
        {
            int expireMinutes = 1;//默认5分钟
            var symmetricKey = Convert.FromBase64String(Constants.ShareCodeKey);
            var now = DateTime.UtcNow;
            var s = now.AddMinutes(Convert.ToInt32(expireMinutes));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                    new Claim(ClaimTypes.Name,username),//
                    new Claim(JwtClaimTypes.Name,username),//指代物主
                    new Claim(JwtClaimTypes.ClientId,DeviceID),//指代被分享的设备
                    new Claim(JwtClaimTypes.Role,"role"),
                    new Claim(JwtClaimTypes.Audience,Constants.Audience),
                    new Claim(JwtClaimTypes.Issuer,Constants.Issuer),

                        }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature),


            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var code = tokenHandler.WriteToken(stoken);

            return code;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Constants.SecretKey);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
        
                return principal; 
            }

            catch (Exception )
            {
                return null;
            }
        }

        /// <summary>
        /// 获取请求中的*
        /// </su mmary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public static string GetRequestTokenString(HttpRequest httpRequest)
        {
            try
            {
                StringValues JwtBearer;
                httpRequest.Headers.TryGetValue("Authorization", out JwtBearer);
                string JwtBearerString = JwtBearer.ToString();
                string[] sArray = JwtBearerString.Split(' ');
                string acToken = sArray[1];//分离出Token
                return acToken;
            }
            catch (Exception e)
            {

                return null;
            }

           

        }

        public static bool ShareCodeCheck(string token)
        {
    
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return false;

                var symmetricKey = Convert.FromBase64String(Constants.ShareCodeKey);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var time = jwtToken.Payload.Exp;

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

                //考虑如何让token立马失效


                return true;

            }

            catch (Exception)
            {
                return false;
            }
        }

    }
}
