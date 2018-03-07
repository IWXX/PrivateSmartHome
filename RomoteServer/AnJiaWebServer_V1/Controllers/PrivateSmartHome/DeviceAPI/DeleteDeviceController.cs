using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnJiaWebServer_V1.Controllers.PrivateSmartHome
{
    [Produces("application/json")]
    [Route("api/DeleteDevice")]
    public class DeleteDeviceController : Controller
    {
        [HttpDelete]
        public JObject Delete()//将设备解绑删除
        {

            JObject result;
            #region 具体逻辑

            //待写


            #endregion
            ErrorRootobject errorRootobject = new ErrorRootobject()
            {
                error_code = "11111",
                msg = "DeviceInfo Delete Successful"
            };

            string serial = JsonConvert.SerializeObject(errorRootobject);


            result = (JObject)JsonConvert.DeserializeObject(serial);

            //
            return result;
        }
    }
}
