using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiServer.Controllers.SmartLock
{
    [Produces("application/json")]
    [Route("PrivateSmartHome/SmartLock")]//改这里可以更改路径
    public class SmartLockController : Controller
    {
        // GET: api/SmartLock
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SmartLock/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/SmartLock
        [HttpPost]
        public object Post([FromBody]object value)
        {
            return value;
        }
        
        // PUT: api/SmartLock/5
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
