using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome.ShareAPI
{
    [Produces("application/json")]
    [Route("PrivateSmartHome/api/V1/[controller]")]
    public class GetShareDeviceListController : Controller
    {
 
        // POST: api/GetShareDeviceList
        [HttpPost]
        public void Post([FromBody]object value)
        {
            JObject result;
            //获取

        }

    }
}
