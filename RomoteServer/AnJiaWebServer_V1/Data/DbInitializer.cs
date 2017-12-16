using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnJiaWebServer_V1.Models;


namespace AnJiaWebServer_V1.Data
{
    public class DbInitializer
    {
        public static void Initialize(AnJiaContext context)
        {
            context.Database.EnsureCreated();//创建是数据库

            #region 初始化User表
            if (context.Users.Any())
            {
                return;   // DB has been seeded 数据库已经被播种
            }

            var users = new Users[]
            {
                    new Users { Username = "yuancong",   Password = "123456",
                        EnrollmentDate = DateTime.Parse("2017-11-14") },
                       new Users
                    {
                        Username="wangchao",Password="123456",
                        EnrollmentDate=DateTime.Parse("2017-11-14")
                    },
                      new Users
                    {
                        Username="yuancong314",Password="t_123456@",
                        EnrollmentDate=DateTime.Parse("2017-11-14")
                    }


            };

            foreach (Users s in users)//将每个对象数组的元素添加到上下文
            {
                context.Users.Add(s);
            }
            context.SaveChanges();

            #endregion

            #region 初始化SubServer表
            if (context.Subservers.Any())
            {
                return;
            }
            else
            {
                var subServers = new SubServers[]
           {
                    new SubServers { SubServerMAC= "00-23-5A-15-99-42", BindingDate = DateTime.Parse("2017-11-01") },
                    new SubServers { SubServerMAC= "00-23-5A-15-99-43", BindingDate = DateTime.Parse("2017-11-02") },
                    new SubServers { SubServerMAC= "00-23-5A-15-99-44", BindingDate = DateTime.Parse("2017-11-03") },
                    new SubServers { SubServerMAC= "00-23-5A-15-99-45", BindingDate = DateTime.Parse("2017-11-04") },
                    new SubServers { SubServerMAC= "00-23-5A-15-99-46", BindingDate = DateTime.Parse("2017-11-05") },
                    new SubServers { SubServerMAC= "00-23-5A-15-99-47", BindingDate = DateTime.Parse("2017-11-06") },
                    new SubServers { SubServerMAC= "00-23-5A-15-99-48", BindingDate = DateTime.Parse("2017-11-07") }
           };

                foreach (SubServers i in subServers)
                {
                    context.Subservers.Add(i);
                }
                context.SaveChanges();
            }
           
            #endregion

            #region 初始化Groups表
            var groups = new Groups[]
            {
                    new Groups { GroupName = "room1",   CreatDate = DateTime.Parse("2007-09-01")},
                    new Groups { GroupName = "room2",   CreatDate = DateTime.Parse("2007-09-01")},
                    new Groups { GroupName = "room3",   CreatDate = DateTime.Parse("2007-09-01")},
                    new Groups { GroupName = "room4",   CreatDate = DateTime.Parse("2007-09-01")},
                    new Groups { GroupName = "room5",   CreatDate = DateTime.Parse("2007-09-01")},

            };

            foreach (Groups d in groups)
            {
                context.Groups.Add(d);
            }
            context.SaveChanges();
            #endregion

            #region 初始化 Enrollment表
            var enrollments = new Enrollment[]
            {
            new Enrollment{UserId=1,GroupId=1},
            new Enrollment{UserId=1,GroupId=1},
            new Enrollment{UserId=1,GroupId=1},
            new Enrollment{UserId=1,GroupId=1},
            new Enrollment{UserId=1,GroupId=1},
            new Enrollment{UserId=1,GroupId=1},
            new Enrollment{UserId=1,GroupId=1},
            };
            foreach (Enrollment e in enrollments)
            {
                context.Enrollments.Add(e);
            }
            context.SaveChanges();
            #endregion

            #region 初始化 UserToSubServer




            #endregion

        }
    }
}
