using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AnJiaWebServer_V1.Models;
using System.Text.RegularExpressions;
using AnJiaWebServer_V1.JSON;
using AnJiaWebServer_V1.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
//用于用户注册
namespace AnJiaWebServer_V1.Controllers
{
    [Route("api/[controller]")]
    public class RegisterController : Controller
    {

        private readonly AnJiaContext anJiaContext;


        public RegisterController(AnJiaContext context)
        {
            anJiaContext = context;
        }


        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "请使用POST方法" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "请使用POST方法";
        }

        // POST api/values
        [HttpPost]
        public async Task<JObject> PostAsync([FromBody]object  value)
        {
            #region 变量声明以及初始化
            string username = "username";
            string password = "password";
            string email = "email";
            string phonenum = "phonenum";
            string sex = "sex";
            JObject regform = (JObject)value;//接收注册表单
            JObject result;//返回结果
            ErrorRootobject error = new ErrorRootobject();
            error.error_code = "00001";
            error.msg = "JSON format error";

            string serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串

            result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
            #endregion

            #region 检查表单必填项
            try
            {

                username = regform["username"].ToString();
                password = regform["password"].ToString();
                email = regform["email"].ToString();
                phonenum = regform["phonenum"].ToString();
                sex = regform["sex"].ToString();
            }
            catch (Exception)
            {
                error.error_code = "0009";
                error.msg = "JSON format error";
                serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象

                return result;
            }

            if (username == "" || password=="")
            {
                error.error_code = "00009";
                error.msg = "Username or password can not be null";
                serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            #endregion

            #region 检查危险字符
            bool unameDanger = Regex.IsMatch(username, @"[-|;|,|\/|||||\}|\{|%|@|\*|!|\']");
            bool pwdDanger = Regex.IsMatch(username, @"[-|;|,|\/|||||\}|\{|%|@|\*|!|\']");
            bool emailCheck = Regex.IsMatch(email, @"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$");//检查邮箱格式
            bool phonnumCheck = Regex.IsMatch(phonenum, @"^1[3|4|5|7|8][0-9]{9}$");//检查手机号格式

            if (unameDanger)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    error_code = "0002",
                    msg = "Username contains dangerous characters "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            if (pwdDanger)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    error_code = "0003",
                    msg = "Password contains dangerous characters "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }

            if (!emailCheck)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    error_code = "0006",
                    msg = "The e-mail address format is incorrect"
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }

            if (!phonnumCheck)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    error_code = "0007",
                    msg = "The phone number format is incorrect"
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            #endregion

            #region 查询用户名是否存在
            var conn = anJiaContext.Database.GetDbConnection();
            conn.Open();
            var command = conn.CreateCommand();
            string query = "SELECT Username "
                + "FROM Users "
                + "WHERE Username = '" + username + "'";
            command.CommandText = query;
            DbDataReader unameReader = await command.ExecuteReaderAsync();


            //用户名存在
            if (unameReader.HasRows)
            {
                //用户名已经被注册
                error = new ErrorRootobject
                {
                    error_code = "0008",
                    msg = "Username has been registered"
                };
                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象   
                conn.Close();
                unameReader.Dispose();//释放资源
                return result;
            }
            #endregion
            conn.Close();
            #region 注册用户
            conn.Open();
            string registDate = DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.TimeOfDay.ToString();
            query = "INSERT INTO Users (Email, Password, Phonenum, RegistDate, Sex, Username)"
               + "VALUES( '"
               + email + " ' ,  '"
               + password + " ' ,  '"
               + phonenum + " ' ,  '"
               + registDate + " ' ,  '"
               + sex + " ' ,  '"
               + username + " '  ) ";
            command.CommandText = query;
            try
            {
                DbDataReader reader = await command.ExecuteReaderAsync();//默认值
                error = new ErrorRootobject
                {
                    error_code = "0000",
                    msg = "Login Successful"
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                conn.Close();
                reader.Dispose();//释放资源
            }
            catch (Exception)
            {

                conn.Close();

                error = new ErrorRootobject
                {
                    error_code = "0011",
                    msg = "Registration failed"
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            

            //密码匹配之后返回J

    
        
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
