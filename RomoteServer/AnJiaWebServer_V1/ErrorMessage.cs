using AnJiaWebServer_V1.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1
{
    public static class ErrorMessage
    {




    }
    public class RegistError
    {
    }
    public class LoginError
    {
    }

    public class ErrorRootobject
    {
        public string ReturnCode { get; set; }//返回码 代表一些
        public string msg { get; set; }
    }
}
