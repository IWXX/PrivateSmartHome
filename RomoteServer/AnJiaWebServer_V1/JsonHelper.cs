using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1
{
    public static class JsonHelper
    {

        //构思
        // 生成Json格式的返回
        //统一Json返回格式，方便使用


        public static JObject JResult(string returnCode,string returnMsg)//返回x
        {
            ErrorRootobject JMsg= new ErrorRootobject()
            {
                ReturnCode = returnCode,
                msg=returnMsg
            };
            string serial = JsonConvert.SerializeObject(JMsg);
            JObject result = (JObject)JsonConvert.DeserializeObject(serial);

            return result;
        }


    }
}
