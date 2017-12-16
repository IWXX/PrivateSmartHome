using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1.Models
{
    public class UserToSubserver
    {
        public int UserToSubserverId { get; set; }
        public string Username { get; set; }
        public string SubServerID { get; set; }
        public DateTime UTSBindingTime { get; set; }
    }
}
