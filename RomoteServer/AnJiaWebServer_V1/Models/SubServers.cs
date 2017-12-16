using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1.Models
{
    public class SubServers
    {
        #region Model
        /// <summary>
        /// 子服务器的ID
        /// </summary>
        public int SubServersID { get; set; }
        /// <summary>
        /// 子服务器的MAC地址
        /// </summary>
        public string SubServerMAC{ get; set; }
        /// <summary>
        /// 用于标识该子服务器已被绑定
        /// </summary>
        public bool SubSrverBound { get; set; }
        /// <summary>
        /// 绑定日期
        /// </summary>
        public DateTime BindingDate { get; set; }
        #endregion
    }
}
