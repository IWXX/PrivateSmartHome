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
        public string error_code { get; set; }
        public string msg { get; set; }
    }
}
