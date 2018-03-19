using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome.ShareAPI
{
    [Produces("application/json")]
    [Route("PrivateSmartHome/api/V1/[controller]")]
    public class GetShareDeviceListController : Controller
    {
        // GET: api/GetShareDeviceList
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GetShareDeviceList/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/GetShareDeviceList
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/GetShareDeviceList/5
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
