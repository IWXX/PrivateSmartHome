using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using AnJiaWebServer_V1.JSON;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Data.Common;
using AnJiaWebServer_V1.Data;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AnJiaWebServer_V1.Controllers
{
    [Route("api/[controller]")]
    public class UpdateUinfoController : Controller
    {
        private readonly AnJiaContext anJiaContext;

        public UpdateUinfoController(AnJiaContext context)
        {
            anJiaContext = context;
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public async Task<JObject> PostAsync([FromBody]object value)
        {
            #region 变量声明以及初始化
            JObject regform = (JObject)value;
            JObject result;//返回结果
            ErrorRootobject error = new ErrorRootobject
            {
                error_code = "00001",
                msg = "JSON format error"
            };
            string serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串
            result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
            string acToken = "token";
            string email = "email";
            string phonenum = "phonenum";
            string sex = "sex";
            string username = "username";
            #endregion

            #region 检查表单
            try
            {
                acToken = regform["AccessToken"].ToString();
                email = regform["Email"].ToString();
                phonenum = regform["Phonenum"].ToString();
                sex = regform["Sex"].ToString();
            }
            catch (Exception)
            {
                error.error_code = "0009";
                error.msg = "JSON format error";
                serial = JsonConvert.SerializeObject(error);//将实体类序列化为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象

                return result;
            }
            #endregion

            #region 排查危险字符
         ;
            bool emailCheck = Regex.IsMatch(email, @"^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$");//检查邮箱格式
            bool phonnumCheck = Regex.IsMatch(phonenum, @"^1[3|4|5|7|8][0-9]{9}$");//检查手机号格式
            bool sexCheck= Regex.IsMatch(sex, @"[-|;|,|\/|||||\}|\{|%|@|\*|!|\']");
            bool tokenCheck = Regex.IsMatch(acToken, @"[|;|,|\/|||||\}|\{|%|@|\*|!|\']");

            bool[] check = { emailCheck,phonnumCheck,sexCheck,tokenCheck};
            List<KeyValuePair<String, Boolean>> paraList = new List<KeyValuePair<string, bool>>
            {
                new KeyValuePair<string, bool>("1003", tokenCheck),
                new KeyValuePair<string, bool>("0006", emailCheck),
                new KeyValuePair<string, bool>("0011", sexCheck),
            };
            foreach (var i in paraList)
            {
                if (i.Value)
                {
                    //失败后返回错误原因：
                    error = new ErrorRootobject
                    {
                        error_code = i.Key,
                        msg = " contains dangerous characters "
                    };

                    serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串
                    result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                    return result;
                }
            }


            #endregion

            #region 检查Token
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

                unameReader.Read();

                username = unameReader["Username"].ToString();
                conn.Close();

            }
            catch (Exception)
            {
                error = new ErrorRootobject
                {
                    error_code = "1004",
                    msg = " Token does not exist "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                conn.Close();
                return result;

            }
            



            #endregion

            #region 更新信息
     
             command = conn.CreateCommand();
             conn.Open();
             query = "UPDATE Users "
                + " SET  email = '" + email + "', "
                + " phonenum = '" + phonenum + "', "
                + " Sex = '" + sex + "' "
                + " WHERE Username = '" + username + "'";
            command.CommandText = query;
            try
            {
                DbDataReader BindingReader = await command.ExecuteReaderAsync();
                error = new ErrorRootobject
                {
                    error_code = "0000",
                    msg = "Update success"
                };
                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                conn.Close();
                BindingReader.Dispose();//释放资源
            }
            catch (Exception)
            {

                //绑定失败
                error = new ErrorRootobject
                {
                    error_code = "0012",
                    msg = "Update failed"
                };
                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                conn.Close();

            }


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
