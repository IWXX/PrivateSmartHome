using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnJiaWebServer_V1.JSON;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome.AccountAPI
{
    /// <summary>
    /// 接口说明
    /// 
    /// 验证分享码
    /// 返回设备列表
    /// </summary>
    [Produces("application/json")]
    [Route("PrivateSmartHome/api/V1/[controller]")]
    public class ShareController : Controller
    {

        /// <summary>
        /// 传入参数 用户名 和 ShareCode
        /// 
        /// Device的分享是一对多的
        /// 在Redis中建立一个关系 Device=>{ user1,user2,user3  }
        /// 其中列表为Device分享给的用户列表
        /// 设备主可以通过修改对应关系来进行增加或者撤销分享
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public JObject Post([FromBody]object value)
        {
            JObject regform = (JObject)value;
            string ReturnCode = "0000";
            string msg = "Share Success";
            try
            {
                var code = regform["shareCode"].ToString();

                //首先要验证ShareCode是否有效
               if(JwtManager.ShareCodeCheck(code))
                {
                    //验证成功 
                    //进行数据库的操作 这个还没想好

                    //返回成功信息

                    return JsonHelper.JResult(ReturnCode,msg);

                }
                else
                {
                    ReturnCode = "0001";
                    msg = "Share fail";
                    return JsonHelper.JResult(ReturnCode,msg);
                }
            }
            catch (Exception e)
            {

                return  JsonHelper.JResult("0001","Excption: "+e.Message);
            }




        

        }
       
    }
}
