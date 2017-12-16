using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1.Models
{
    public class Users
    {
        public int UsersId{ get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 注册用的电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Phonenum { get; set; }
        /// <summary>
        /// 用户所属组id
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 与组绑定的日期
        /// </summary>
        public DateTime EnrollmentDate { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegistDate { get; set; }
        /// <summary>
        /// 该用户的Token
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 绑定的子服务器MAC
        /// </summary>
        public string SubserverMAC { get; set; }
        /// <summary>
        /// 头像路径
        /// </summary>
        public string UserAvatarPath { get; set; }


        public ICollection<Enrollment> Enrollments { get; set; }

    }
}
