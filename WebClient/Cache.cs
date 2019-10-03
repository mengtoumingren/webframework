using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebClient
{

    public class Cache
    {
        class CacheData
        {
            public object Data { get; set; }
            /// <summary>
            /// 过期时间
            /// </summary>
            public DateTime ExpiredTime { get; set; }
            /// <summary>
            /// 是否绝对时间
            /// </summary>
            public bool IsAbsoluteTime { get; set; }
            /// <summary>
            /// 过期间隔
            /// </summary>
            public TimeSpan? ExpiredTimeSpan { get; set; }
        }
        private static Dictionary<string, CacheData> cacheValues;
        private static object objLock;
        static Cache()
        {
            cacheValues = new Dictionary<string, CacheData>();
            objLock = new object();
        }

        /// <summary>
        /// 自动删除缓存任务，定时删除过期数据
        /// </summary>
        private static void RemoveTask()
        {
            while(true)
            {
                Thread.Sleep(100);
                foreach (var key in cacheValues.Keys)
                {
                    try
                    {
                        var data = cacheValues[key];
                        if (data.ExpiredTime < DateTime.Now)
                        {
                            Remove(key);
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                }
                Thread.Sleep(60000);
            }
        }

        public object this[string name] { get => Get(name); set => Add(name, value, TimeSpan.FromMinutes(30)); }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="key">关键词</param>
        /// <param name="value">值</param>
        /// <param name="ExpiredTime">过期时间 非绝对时间不能为空</param>
        /// <param name="AbsoluteTime">是否绝对时间</param>
        /// <param name="AbsoluteExpiredTime">绝对过期时间 是绝对时间时不能为空</param>
        public static void Add(string key, object value, TimeSpan? ExpiredTimeSpan = null, bool AbsoluteTime = false, DateTime? AbsoluteExpiredTime = null)
        {
            lock (objLock)
            {
                var data = new CacheData();
                data.IsAbsoluteTime = AbsoluteTime;
                data.ExpiredTimeSpan = ExpiredTimeSpan;
                if (AbsoluteTime)
                {
                    if (AbsoluteExpiredTime == null) throw new Exception("AbsoluteExpiredTime can not be null ");
                    data.ExpiredTime = AbsoluteExpiredTime.Value;
                }
                else
                {
                    if (ExpiredTimeSpan == null) throw new Exception("ExpiredTimeSpan can not be null ");
                    data.ExpiredTime = DateTime.Now.Add(ExpiredTimeSpan.Value);
                }

                if (cacheValues.ContainsKey(key))
                {
                    cacheValues[key] = data;
                }
                else
                {
                    cacheValues.Add(key, data);
                }
            }
        }
        /// <summary>
        /// 获取缓存，如果缓存过期则移除，如果缓存是非绝对过期时间，则更新过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public static object Get(string key)
        {
            if (cacheValues.ContainsKey(key))
            {
                lock (objLock)
                {
                    if (cacheValues.ContainsKey(key))
                    {
                        var data = cacheValues[key];
                        if(data.ExpiredTime>=DateTime.Now)
                        {
                            if (!data.IsAbsoluteTime)
                                data.ExpiredTime = DateTime.Now.Add(data.ExpiredTimeSpan.Value);
                            return data.Data;
                        }
                        else
                        {
                            cacheValues.Remove(key);
                        }
                    }
                }
            }
            return null;
        }
        
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Remove(string key)
        {
            if (cacheValues.ContainsKey(key))
            {
                lock(objLock)
                {
                    if (cacheValues.ContainsKey(key))
                    {
                        var data = cacheValues[key];
                        cacheValues.Remove(key);
                        return data;
                    }
                }
            }
            return null;
        }

    }
}
