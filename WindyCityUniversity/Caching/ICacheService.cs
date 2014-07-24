using System;

namespace WindyCityUniversity.Caching
{
    interface ICacheService
    {
        T Get<T>(string cacheID, Func<T> getItemCallback) where T : class;
    }
}
