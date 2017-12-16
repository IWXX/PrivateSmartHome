using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1.Models
{
    public class UserAndSubUser
    {
        public int UserAndSuUserID { get; set; }
        public string UserName { get; set; }
        public string SubUserName { get; set; }
        public DateTime UWSBindingTime { get; set; }

    }
}
