using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AnJiaWebServer_V1.JSON;
using AnJiaWebServer_V1.Models;
using AnJiaWebServer_V1.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
//用于接收绑定子服务器 的请求
namespace AnJiaWebServer_V1.Controllers
{
    [Route("api/[controller]")]
    public class SubServerBindController : Controller
    {
        private readonly AnJiaContext anJiaContext;


        public SubServerBindController(AnJiaContext context)
        {
            anJiaContext = context;
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "请使用POST方法" };
        }

 
        // POST api/values
        [HttpPost]
        public async Task<JObject> PostAsync([FromBody]object value)
        {
  
            #region 变量声明以及初始化
            JObject jObject = (JObject)value;
            JObject result;
            string acToken;
            string subServerId; ;
            string username = "username";
            bool acTokenDanger = false;
            bool macDanger = false;
            bool macAvailable = false;


            ErrorRootobject error = new ErrorRootobject
            {
                error_code = "0001",
                msg = "JSON format error"
            };
            string serial = JsonConvert.SerializeObject(error);
            result = (JObject)JsonConvert.DeserializeObject(serial);
            #endregion

            #region 获取JSON内容
            try
            {
                acToken = jObject["actoken"].ToString();
                subServerId = jObject["subserverId"].ToString();
                acTokenDanger = Regex.IsMatch(acToken, @"[|;|,|\/|||||\}|\{|%|@|\*|!|\']");//排查危险字符
                macDanger = Regex.IsMatch(subServerId, @"[|;|,|\/|||||\}|\{|%|@|\*|!|\']");
                macAvailable = true;
            }
            catch (Exception)
            {
                error.error_code = "1001";
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
                    error_code = "1003",
                    msg = "Token contains dangerous characters "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            if (macDanger)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    error_code = "1002",
                    msg = "MAC contains dangerous characters "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }
            #endregion

            #region 检查MAC是否合格

            if (!macAvailable)
            {
                //失败后返回错误原因：
                error = new ErrorRootobject
                {
                    error_code = "1011",
                    msg = "MAC is not available "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }

            #endregion

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
                    error_code = "1004",
                    msg = " Invalid access_Token "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串
                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;

            }
            #endregion

            #region 查询MAC是否已经被绑定
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
                string buser = BindedReader["Username"].ToString();
                error.error_code = "1005";
                error.msg = " This Subserver is owned by "+buser;

                serial = JsonConvert.SerializeObject(error);
                result = (JObject)JsonConvert.DeserializeObject(serial);
                conn.Close();//关闭连接
                return result;
               
            }
            conn.Close();

            #endregion

            #region 绑定MAC
            conn.Open();
            string UTSBindingTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.TimeOfDay.ToString();
            query = "INSERT INTO UserToSubserver (SubServerID, UTSBindingTime,Username)"
               + "VALUES( '" 
               +subServerId+"', '"
               +UTSBindingTime+"', '"
               +username
               +"'  ) ";
            command.CommandText = query;
            try
            {
                DbDataReader BindingReader = await command.ExecuteReaderAsync();
                error = new ErrorRootobject
                {
                    error_code = "0008",
                    msg = "SubServer binding success"
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
                    error_code = "0009",
                    msg = "SubServer binding failed"
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
