using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnJiaWebServer_V1.JSON;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome
{
    [Produces("application/json")]
    [Route("PrivateSmartHome/api/V1/[controller]")]
    public class GetSubserverListController : Controller
    {
        // GET: api/GetSubserverList
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GetSubserverList/5
        [HttpGet("{id}", Name = "username1")]
        public async Task<JObject> Get(string username)//通过用户名来获取设备列表
        {

            JObject result = null;

            #region 从数据库中查询出该用户的子服务器列表

            //待写
            #endregion


            //模拟查询后的结果
            string serial = JsonConvert.SerializeObject(GetTestSubserverList());
            result = (JObject)JsonConvert.DeserializeObject(serial);

            return result;
        }

        private SubserverList GetTestSubserverList()
        {
            int count = 2;
            SubserverList subserverList = new SubserverList(count);


            for (int i = 0; i <= 1; i++)
            {
                subserverList.SubserverInfo[i] = new SubserverInfo()
                {
                    SubserverID = "00000" + i,////可以考虑使用密码学方面无法伪造的ID
                    SubserverIP = "192.168.1.1" + i,
                    SubserverNickName = "树莓派" + i
                };
            }

            return subserverList;
        }

        // POST: api/GetSubserverList
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/GetSubserverList/5
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
