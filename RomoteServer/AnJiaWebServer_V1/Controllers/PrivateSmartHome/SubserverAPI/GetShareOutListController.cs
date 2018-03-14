using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome.SubserverAPI
{
    [Produces("application/json")]
    [Route("api/GetShareOutList")]
    public class GetShareOutListController : Controller
    {
        /// <summary>
        /// 用户一方面需要知道设备分享给了哪些人
        /// 另一方面用户也需要知道我有多少个设备是分享来的
        /// </summary>
        /// <returns></returns>
        // GET: api/GetShareOutList
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GetShareOutList/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/GetShareOutList
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/GetShareOutList/5
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
