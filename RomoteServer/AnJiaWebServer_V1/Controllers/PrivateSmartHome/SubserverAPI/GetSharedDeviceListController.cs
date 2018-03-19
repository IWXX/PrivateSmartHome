using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome.SubserverAPI
{
    [Produces("application/json")]
    [Route("api/GetSharedDeviceList")]
    public class GetSharedDeviceListController : Controller
    {
        // GET: api/GetSharedDeviceList
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GetSharedDeviceList/5
        [HttpGet("{if}")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/GetSharedDeviceList
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/GetSharedDeviceList/5
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
