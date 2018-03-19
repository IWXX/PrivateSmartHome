using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AnJiaWebServer_V1.Data;
using AnJiaWebServer_V1.JSON;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using AnJiaWebServer_V1;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AnJiaWebServer_V1.Controllers
{
   /// <summary>
   /// 共享的设备控制逻辑
   /// 
   /// 
   /// 添加一个字段；指示，是否是共享状态，
   /// 
   /// 如果是，验证sharecode是否有效，有效 则落实控制操作
   /// 
   /// 如果不是共享状态，无需验证 shareCode 走正常流程
   /// </summary>
    
    [Route("PrivateSmartHome/api/V1/[controller]")]

    
    public class OpenLockController : Controller
    {
        private readonly AnJiaContext anJiaContext;
      
        public OpenLockController(AnJiaContext context)
        {
            anJiaContext = context;
            
        }


        // POST api/values
        [HttpPost]
        public async Task<JObject> PostAsync([FromBody]object value)
        {
            #region 注销检测
            string token = JwtManager.GetRequestTokenString(Request);
            var redis = RedisHelper.GetRedisHelper();
            if (!redis.SignInCheck(token))
            {
                return null;//返回错误信息提示重新登录
            }
            #endregion

            #region 变量声明以及初始化
            JObject jObject = (JObject)value;
            JObject result;
            string acToken;
            string subServerId ;
            string device_Ip;
            string username = "username";
            bool acTokenDanger = false;
            bool subserveripDanger = false;
            bool deviceipDanger = false;
            bool subserveridAvailable = false;
            bool deviceipAvailable = false;
            string buser1="buser";
            string buser2;

            ErrorRootobject error = new ErrorRootobject
            {
                ReturnCode = "0001",
                msg = "JSON format error"
            };
            ControlMsgRootobject controlMsg = new ControlMsgRootobject
            {
                DeviceIP = "F0-79-59-17-58-B7"
            };
            string serial = JsonConvert.SerializeObject(error);
            result = (JObject)JsonConvert.DeserializeObject(serial);
            #endregion

            #region 获取JSON内容
            try
            {
                acToken = jObject["actoken"].ToString();
                subServerId = jObject["subserverId"].ToString();
                device_Ip = jObject["deviceIP"].ToString();
                acTokenDanger = Regex.IsMatch(acToken, @"[|;|,|\/|||||\}|\{|%|@|\*|!|\']");//排查危险字符
                subserveripDanger = Regex.IsMatch(subServerId, @"[|;|,|\/|||||\}|\{|%|@|\*|!|\']");
                subserveridAvailable = true;
                deviceipDanger = Regex.IsMatch(subServerId, @"[|;|,|\/|||||\}|\{|%|@|\*|!|\']");
                deviceipAvailable = Regex.IsMatch(device_Ip, @"((25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))");
            }

            catch (Exception)
            {
                error.ReturnCode = "2009";
                error.msg = "JSON format is incorrect";
                serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;

            }
            #endregion

            #region 检查危险字符
            if (acTokenDanger)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    ReturnCode = "2003",
                    msg = "Token contains dangerous characters "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            if (subserveripDanger)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    ReturnCode = "2002",
                    msg = "subserverid contains dangerous characters "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            if (deviceipDanger)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    ReturnCode = "2008",
                    msg = "deviceip contains dangerous characters "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }

            #endregion

            #region 检查ip是否合格

            if (!subserveridAvailable)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    ReturnCode = "2005",
                    msg = "subserverid is not available "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            if (!deviceipAvailable)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    ReturnCode = "2006",
                    msg = "deviceIP is not available "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }

            #endregion

            controlMsg.DeviceIP = device_Ip;

            #region 查询Token是否有效
            var conn = anJiaContext.Database.GetDbConnection();
            conn.Open();
            var command = conn.CreateCommand();
            string query = "SELECT Username "
                + "FROM Users "
                + "WHERE AccessToken = '" + acToken + "'";
            command.CommandText = query;
            try
            {
                DbDataReader unameReader = await command.ExecuteReaderAsync();
                unameReader.Read();//Read must be called first
                username = unameReader["Username"].ToString();
                conn.Close();

            }
            catch (Exception)
            {
                conn.Close();
                error = new ErrorRootobject
                {
                    ReturnCode = "1004",
                    msg = " Invalid access_Token "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;

            }
            #endregion

            #region 查询子服务器MAC对应的用户名
            //查询MAC是否匹配有Username保证了一个MAC只对应一个User
            conn = anJiaContext.Database.GetDbConnection();
            conn.Open();
            command = conn.CreateCommand();
            query = "SELECT  Username"
                + " FROM UserToSubserver "
                + "WHERE SubserverID = '" + subServerId + "'";
            command.CommandText = query;
            DbDataReader BindedReader = await command.ExecuteReaderAsync();
            if (BindedReader.HasRows)
            {
                //如果查询到被绑定
                //我担心会查出一个MAC地址绑定了多个用户的情况
                //后期可以设置让MAC地址为主键
                BindedReader.Read();
                buser1 = BindedReader["Username"].ToString();

            }
            conn.Close();

            #endregion

            #region Token对应的用户名

            //查询MAC是否匹配有Username保证了一个MAC只对应一个User
            conn = anJiaContext.Database.GetDbConnection();
            conn.Open();
            command = conn.CreateCommand();
            query = "SELECT  Username"
                + " FROM Users "
                + "WHERE AccessToken = '" + acToken + "'";
            command.CommandText = query;
            DbDataReader BUserReader = await command.ExecuteReaderAsync();
            if (BUserReader.HasRows)
            {
                //如果查询到被绑定
                //我担心会查出一个MAC地址绑定了多个用户的情况
                //后期可以设置让MAC地址为主键
               BUserReader.Read();
                buser2 = BUserReader["Username"].ToString();

                //如果子服务器对应的用户名和token对应的用户名相同
                if (buser1 == buser2)
                {

                    bool sendSuccess=   await WebsocketClient.SendToSubserverAsync(subServerId, controlMsg);//发送给指定MAC信息
                    if (sendSuccess)
                    {
                        error.ReturnCode = "2000";
                        error.msg = " ControlMsg send success";

                        serial = JsonConvert.SerializeObject(error);
                        result = (JObject)JsonConvert.DeserializeObject(serial);
                    }
                    else
                    {
                        //
                        error.ReturnCode = "2001";
                        error.msg = "  Subserver offline";
                        serial = JsonConvert.SerializeObject(error);
                        result = (JObject)JsonConvert.DeserializeObject(serial);

                        //接下来对表进行更改
                    }

                    conn.Close();//关闭连接
                    return result;
                }
                else
                {
                    //提示没有对此服务器操作权限
                    error.ReturnCode = "2007";
                    error.msg = " You do not have permission to operate this subserver ";

                    serial = JsonConvert.SerializeObject(error);
                    result = (JObject)JsonConvert.DeserializeObject(serial);
                    conn.Close();//关闭连接
                    return result;
                }

            }
            conn.Close();



            #endregion

            return result;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
