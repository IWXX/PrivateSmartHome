using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1.Models
{
    public class Groups
    {
        /// <summary>
        /// 组id
        /// </summary>
        public int GroupsID { get; set; }
        /// <summary>
        /// 组名（由用户设置）
        /// </summary>
        public string GroupName { get; set; }
        public DateTime CreatDate { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
