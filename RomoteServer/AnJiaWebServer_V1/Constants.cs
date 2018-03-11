using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1
{
    public static class Constants
    {
        public const string SecretKey= "4IZy0vYvvKofcx1Kn5UUByRqoZR5Q4dVWNbHd82tOxhTPaMd770sr9BzF7dgPWPO2kvFhO5fXCea6gedc2LrEg==";
        public const string ShareCodeKey = "eglfMr6yKrggA+1anyy/Gjg4SblgRzNwvtkHAW7nXgZxNX0cW/YDeWYBnRm5XAiYSY71nvx7xmNuU2z3dxVCng==";
        public const string Audience = "PrivateSmartHome";
        public const string Issuer = "AnJia";
        public const string RedisCon = "127.0.0.1:6379";//Redis连接字符串

    }

    public static class ReturnCodes
    {
        //返回码如何编排



        
        public const string SignInSuccess = "1000";
    }
}
