using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;

namespace AnJiaWebServer_V1
{
    public static class JwtManager
    {
        /// <summary>
        /// 使用以下代码来生成对称密钥
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>

        public static string GenerateToken(string username, int expireMinutes = 720)
        {
            var symmetricKey = Convert.FromBase64String(Constants.SecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            var s = now.AddMinutes(Convert.ToInt32(expireMinutes));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                    new Claim(ClaimTypes.Name,"username"),
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

    }
}
