using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AnJiaWebServer_V1.Controllers
{
    /// <summary>
    /// 用户主动登出
    /// </summary>
   // [Authorize]
    [Route("api/[controller]")]
    public class SignOutController : Controller//注销登录操作
    {
        /// <summary>
        /// 接口名：注销API
        /// 功能：对指定用户进行登录状态注销
        /// </summary>
        /// <param name="username"></param>
        /// <returns>注销结果</returns>
    
        [HttpDelete("{username}")]
        public JObject Delete(string username)
        {
         
            #region 注销检测
            var redis = RedisHelper.GetRedisHelper();
            if (!redis.SignInCheck(username))
            {
                //如果访问接口时，该用户已经处于注销状态
                //则返回 提示信息已注销
                return JsonHelper.JResult("3001", "SignOut Repeat");//返回错误信息提示重新登录
            }
            #endregion

            #region 业务逻辑

            redis.DeleteKey(username);//删除用户的Token关联
            #endregion

            return JsonHelper.JResult("3000","SignOut Success");

        }
    }
}
