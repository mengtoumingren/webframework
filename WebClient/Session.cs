using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebClient
{
    public class Session
    {
        private string SessionId { get; set; }
        internal Session(string SessionId)
        {
            this.SessionId = SessionId;
        }

        public object this[string name] { get => Get(name); set => Add(name, value); }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="key">关键词</param>
        /// <param name="value">值</param>
        public void Add(string key, object value)
        {
            Cache.Add(key, value, TimeSpan.FromMinutes(20));
        }
        /// <summary>
        /// 获取缓存，
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>

        public object Get(string key)
        {
            return Cache.Get($"{SessionId}.{key}");
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Remove(string key)
        {
            return Cache.Remove($"{SessionId}.{key}");
        }
    }
}
