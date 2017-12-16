using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1.JSON
{

    public class DeviceList
    {
        public int Devicenum { get; set; }
        public Deviceinfo[] DeviceInfo { get; set; }
    }

    public class Deviceinfo
    {
        public string DeviceType { get; set; }
        public string DeviceIP { get; set; }
        public string SubserverId { get; set; }
        public string DeviceNickname { get; set; }
    }

}
