using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using AnJiaWebServer_V1.JSON;
using Newtonsoft.Json;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome
{
    [Produces("application/json")]
    //   [Authorize]
    [Route("PrivateSmartHome/api/V1/[controller]")]

    public class DevicesController : Controller
    {
        // GET: api/Devics
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Devics/5
        [HttpGet("{id}", Name = "username")]
        public async Task<JObject> Get(string username)//通过用户名来获取设备列表
        {

            JObject result=null;

            #region 从数据库中查询出该用户的设备列表

            //待写
            #endregion


            //模拟查询后的结果
            string serial = JsonConvert.SerializeObject(GetTestDeviceList());
            result = (JObject)JsonConvert.DeserializeObject(serial);

            return result;
        }



        public DeviceList GetTestDeviceList()
        {
            int count = 11;
            DeviceList deviceList = new DeviceList(count);

            for(int i = 0; i <= 10; i++)
            {

                deviceList.DeviceInfo[i] = new Deviceinfo()
                {
                    DeviceID = "00000" + i,
                    DeviceIP = "192.168.1.10" + (i + 1),
                    DeviceNickname = "锁具" + i,
                    DeviceType = "0"//0，代表锁具，1，2，3·····以此类推代表不同的具体设备，这个待定

                };
            
            }

            return deviceList;
        }

        // POST: api/Devics
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Devics/5
        [HttpPut("{id}")] 
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]       
        public void Delete(int id)
        {
        }
    }
}
