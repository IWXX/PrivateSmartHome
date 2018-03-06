using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnJiaWebServer_V1.Data;
using AnJiaWebServer_V1.JSON;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AnJiaWebServer_V1.Controllers
{/// <summary>
 /// 获取用户个人信息
 /// </summary>

    [Authorize]
    [Route("PrivateSmartHome/api/V1/[controller]")]
    public class GetUinfoController : Controller
    {
        private readonly AnJiaContext anJiaContext;
        public GetUinfoController(AnJiaContext context)
        {
            anJiaContext = context;
        }
        // GET: api/values
        [HttpGet("{username}")]
        public async Task<JObject> Get(string username)
        {
            #region 变量声明以及初始化

    
            JObject result;//返回结果
      
            StringValues JwtBearer;
            Request.Headers.TryGetValue("Authorization", out JwtBearer);
            string JwtBearerString = JwtBearer.ToString();
            string[] sArray = JwtBearerString.Split(' ');
            string acToken = sArray[1];//分离出Token

            var claimsPrincipal = JwtManager.GetPrincipal(acToken);//对Token

            string uname = claimsPrincipal.Identity.Name.ToString();//获取用户名

            

            ErrorRootobject error = new ErrorRootobject
            {
                error_code = "00001",
                msg = "JSON format error"
            };

            string serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串

            result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
             username = "username";


            if (uname != username)//提交的用户名与Token不匹配
            {
                ErrorRootobject error1 = new ErrorRootobject
                {
                    error_code = "00001",
                    msg = "User and Token mismatch"
                };

                string serial1 = JsonConvert.SerializeObject(error1);//将实体类序列化为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial1);//将JSON字符串反序列化为JObject对象
                return result;
            }
            #endregion

            #region 检查表单
            try
            {
   

            }
            catch
            {
                error.error_code = "00001";
                error.msg = "JSON format error";

                serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            #endregion

            #region 用户名以及密码的判空
            if (username == "")
            {
                error.error_code = "00009";
                error.msg = "Username can not be null";
                serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            #endregion

            #region 用户名以及密码的危险字符检查
            //排查危险字符
            bool unameDanger = Regex.IsMatch(username, @"[-|;|,|\/|||||\}|\{|%|@|\*|!|\']");

            if (unameDanger)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject();
                error.error_code = "0002";
                error.msg = "Username contains dangerous characters ";

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }

            #endregion

            #region 查询用户信息
            var conn = anJiaContext.Database.GetDbConnection();
         
            conn.Open();
            var command = conn.CreateCommand();
            string query = "SELECT Username,Email,  Phonenum, RegistDate, Sex, Username"
                    + "FROM Users "
                    + "WHERE Username = '" + username + "'";
            command.CommandText = query;
            DbDataReader reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                conn.Close();//关闭连接

                reader.Read();
                username = reader["Username"].ToString();
                string Email = reader["Email"].ToString();
                string Phonenum = reader["Phonenum"].ToString();
                string RegistDate = reader["RegistDate"].ToString();
                var redis = new RedisHelper(Constants.RedisCon);

                error = new ErrorRootobject
                {

                    error_code = "0010",
                    msg = "Token failed to get"
                };
               // serial = JsonConvert.SerializeObject(actoken);//将实体类序列化为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象



            }
            else
            {
                conn.Close();
                //密码不匹配返回错误原因：
                error = new ErrorRootobject();
                error.error_code = "0005";
                error.msg = "Incorrect username or password";

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
            }
            reader.Dispose();//释放资源

            #endregion



            return result;

        }



        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
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
