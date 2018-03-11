
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnJiaWebServer_V1
{
    public class RedisHelper
    {

        private static RedisHelper uniqueInstance;//唯一实例
        private static ConnectionMultiplexer Redis { get; set; }

        private static readonly object locker = new object();//定义一个标识符确保线程同步
        private IDatabase Db { get; set; }
        private RedisHelper(string connection)
        {
            Redis = ConnectionMultiplexer.Connect(connection);
            Db = Redis.GetDatabase();
        }

        public static RedisHelper GetRedisHelper()
        {
            if (uniqueInstance == null)
            {
                lock (locker)
                {
                    uniqueInstance = new RedisHelper(Constants.RedisCon);
                }
            }
            return uniqueInstance;
        }
      


        /// <summary>
        /// 增加/修改
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(string key, string value)
        {
            return Db.StringSet(key, value);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return Db.StringGet(key);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool DeleteKey(string key)
        {
            return Db.KeyDelete(key);
        }

        public bool SignInCheck(string token)//检查登录是否有效
        {
                string value = GetValue(token);
                if (value == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }       
        }

        public bool ShareCheck(string shareCode)//检查分享是否有效
        {



            return false;
        }
    }
}
