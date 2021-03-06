﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome
{
    [Produces("application/json")]
    [Route("api/DeleteDevice")]
    public class DeleteDeviceController : Controller
    {
        [HttpDelete]
        public JObject Delete([FromBody]object value)//将设备解绑删除
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


            #endregion
            ErrorRootobject errorRootobject = new ErrorRootobject()
            {
                ReturnCode = "11111",
                msg = "DeviceInfo Delete Successful"
            };

            string serial = JsonConvert.SerializeObject(errorRootobject);
            result = (JObject)JsonConvert.DeserializeObject(serial);

            return result;
        }
    }
}
