using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1.JSON
{
    public class SubserverList
    {
        public int SubserverNum { get; set; }
        public SubserverInfo[] SubserverInfo { get; set; }


        public SubserverList(int count)
        {
            SubserverNum = count;
            SubserverInfo = new SubserverInfo[count];
        }
    }


    public class SubserverInfo//子服务器属性
    {
        public string SubserverID { get; set; }
        public string SubserverIP { get; set; }
        public string SubserverNickName { get; set; }


    }
}
