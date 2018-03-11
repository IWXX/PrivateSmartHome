using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AnJiaWebServer_V1.JSON;
using System.Text.RegularExpressions;//使用正则表达式对用户输入进行合法性检查
using AnJiaWebServer_V1.Data;
using AnJiaWebServer_V1.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
//用于用户外网登录
namespace AnJiaWebServer_V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("PrivateSmartHome/api/V1/[controller]")]
    public class SignInController : Controller//登录操作
    {
        private readonly AnJiaContext anJiaContext;


        public SignInController(AnJiaContext context)
        {
            anJiaContext = context;
        }

        [HttpPost]   
        public async System.Threading.Tasks.Task<JObject> PostAsync([FromBody]object value)
        {

            #region 变量声明以及初始化
            JObject jObject = (JObject)value;//获取为Json对象
            JObject result;//返回结果


            ErrorRootobject error = new ErrorRootobject();
            error.ReturnCode = "00001";
            error.msg = "JSON format error";

            string serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串

            result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
            string username = "username";
            string password = "password";
            #endregion

            #region 检查表单
            try
            {
                username = jObject["username"].ToString();
                password = jObject["password"].ToString();
            }
            catch
            {
                error.ReturnCode = "00001";
                error.msg = "JSON format error";

                serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            #endregion

            #region 用户名以及密码的判空
            if (username == "" || password == "")
            {
                error.ReturnCode = "00009";
                error.msg = "Username or password can not be null";
                serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            #endregion

            #region 用户名以及密码的危险字符检查
            //排查危险字符
            bool unameDanger = Regex.IsMatch(username, @"[-|;|,|\/|||||\}|\{|%|@|\*|!|\']");
            bool pwdDanger = Regex.IsMatch(password, @"[-|;|,|\/|||||\}|\{|%|@|\*|!|\']");

            if (unameDanger)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject();
                error.ReturnCode = "0002";
                error.msg = "Username contains dangerous characters ";

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            if (pwdDanger)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject();
                error.ReturnCode = "0003";
                error.msg = "Password contains dangerous characters ";

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            #endregion

            #region 检查用户名是否存在
            //将安全的用户名和密码查询数据库
            //首先查询用户名是否存在
            var conn = anJiaContext.Database.GetDbConnection();
            conn.Open();
            var command = conn.CreateCommand();
            string query = "SELECT Username "
                + "FROM Users "
                + "WHERE Username = '" + username + "'";
            command.CommandText = query;
            DbDataReader unameReader = await command.ExecuteReaderAsync();
            //用户名不存在
            if (!unameReader.HasRows)
            {
                conn.Close();

                //密码不匹配返回错误原因：
                error = new ErrorRootobject
                {
                    ReturnCode = "0004",
                    msg = "Username does not exist"
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                unameReader.Dispose();//释放资源
                return result;
            }
            else
            {
                conn.Close();
            }

            #endregion

            #region 用户名与密码匹配验证

            conn.Open();
            query = "SELECT Username, Password "
                    + "FROM Users "
                    + "WHERE Username = '" + username + "'" + " AND password = '" + password + "'";
            command.CommandText = query;
            DbDataReader reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                conn.Close();//关闭连接

                var accessToken = JwtManager.GetJwtManager().GenerateToken(username);//生成Token
                var redis = RedisHelper.GetRedisHelper();

                redis.SetValue(username,accessToken);//在redis中建立用户名和Token的对应关系

                LoginSuccessRootobject actoken = new LoginSuccessRootobject
                {
                    AccessToken = accessToken//获取一个Token
                };

                    error = new ErrorRootobject
                    {

                        ReturnCode = "0010",
                        msg = "Token failed to get"
                    };
                    serial = JsonConvert.SerializeObject(actoken);//将实体类序列化为JSON字符串
                    result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
            }
            else
            {
                conn.Close();
                //密码不匹配返回错误原因：
                error = new ErrorRootobject();
                error.ReturnCode = "0005";
                error.msg = "Incorrect username or password";

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
            }
            reader.Dispose();//释放资源

            #endregion

            return result;
        
    }

    }
}
