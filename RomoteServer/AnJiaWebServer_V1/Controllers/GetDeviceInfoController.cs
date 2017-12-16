using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AnJiaWebServer_V1.JSON;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AnJiaWebServer_V1.Controllers
{
    [Route("api/[controller]")]
    public class GetDeviceInfoController : Controller
    {
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
        public JObject Post([FromBody]object value)
        {

            #region 变量声明以及初始化
            JObject jObject = (JObject)value;
            JObject result;
            string acToken;
            string subServerId;
            string device_Ip;
            string username = "username";
            bool acTokenDanger = false;
            bool subserveripDanger = false;
            bool deviceipDanger = false;
            bool subserveridAvailable = false;
            bool deviceipAvailable = false;
            string buser1 = "buser";
            string buser2;

            ErrorRootobject error = new ErrorRootobject
            {
                error_code = "0001",
                msg = "JSON format error"
            };

            DeviceList deviceList = new DeviceList();
            Deviceinfo deviceinfo = new Deviceinfo
            {
                DeviceIP = "",
                DeviceNickname = "",
                DeviceType = ""
            };
            deviceList.DeviceInfo[0] = deviceinfo;



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
                error.error_code = "2009";
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
                    error_code = "2003",
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
                    error_code = "2002",
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
                    error_code = "2008",
                    msg = "deviceip contains dangerous characters "
                };

                serial = JsonConvert.SerializeObject(error);//将实体类序列化   为JSON字符串

                result = (JObject)JsonConvert.DeserializeObject(serial);//将JSON字符串反序列化为JObject对象
                return result;
            }

            #endregion



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
