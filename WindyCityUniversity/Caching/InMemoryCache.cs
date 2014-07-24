using System;
using System.Web;

namespace WindyCityUniversity.Caching
{
    // courtesy of http://stackoverflow.com/a/349111/276681
    public class InMemoryCache : ICacheService
    {
        public T Get<T>(string cacheID, Func<T> getItemCallback) where T : class
        {
            T item = HttpRuntime.Cache.Get(cacheID) as T;
            if (item == null)
            {
                item = getItemCallback();
                HttpContext.Current.Cache.Insert(cacheID, item);
            }
            return item;
        }
    }
}