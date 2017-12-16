using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace AnJiaWebServer_V1.Models
{
    public class ShareEnable
    {
        //在这个表中的Subserver均是可以共享的
        public int ShareEnableID { get; set; }
        public string Username { get; set; }
        public string SubserverID { get; set; }
    }
}
