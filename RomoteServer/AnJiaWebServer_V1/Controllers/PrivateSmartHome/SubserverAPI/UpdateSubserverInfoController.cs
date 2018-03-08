using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome.SubserverAPI
{
    [Produces("application/json")]
    [Route("PrivateSmartHome/api/V1/[controller]")]
    public class UpdateSubserverInfoController : Controller
    {
        [HttpPut]
        public JObject Post([FromBody]object value)//更改SubServer的信息，比如自定义名称
        {
            JObject result;
            #region 具体逻辑
            //待写
            JObject regform = (JObject)value;//获取Json



            #endregion
            ErrorRootobject errorRootobject = new ErrorRootobject()
            {
                error_code = "11111",
                msg = "DeviceInfo Update Successful"
            };

            string serial = JsonConvert.SerializeObject(errorRootobject);
            result = (JObject)JsonConvert.DeserializeObject(serial);

            return result;
        }

    }
}
