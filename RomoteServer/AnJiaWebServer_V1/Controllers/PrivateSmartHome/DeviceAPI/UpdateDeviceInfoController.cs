using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome
{
  //  [Authorize]
    [Produces("application/json")]
    [Route("PrivateSmartHome/api/V1/[controller]")]
    public class UpdateDeviceInfoController : Controller
    {
        [HttpPut]
        public JObject Post([FromBody]object value)//更改Devices的信息，比如自定义名称
        {
            #region 注销检测
            string token = JwtManager.GetRequestTokenString(Request);
            var redis = RedisHelper.GetRedisHelper();
            if (!redis.SignInCheck(token))
            {
                return null;//返回错误信息提示重新登录
            }
            #endregion
            JObject result;
            #region 具体逻辑

            //待写
            JObject regform = (JObject)value;//获取Json



            #endregion
            ErrorRootobject errorRootobject = new ErrorRootobject()
            {
                ReturnCode = "11111",
                msg = "DeviceInfo Update Successful"
            };

            string serial = JsonConvert.SerializeObject(errorRootobject);
            result = (JObject)JsonConvert.DeserializeObject(serial);

            return result;
        }
    }
}
