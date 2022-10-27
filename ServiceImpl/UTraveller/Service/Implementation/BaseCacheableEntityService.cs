using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Implementation
{
    public class BaseCacheableEntityService
    {
        private IDictionary<long, bool> isGetFromCache;

        public BaseCacheableEntityService()
        {
            isGetFromCache = new Dictionary<long, bool>();
        }


        protected void SetCacheFlag(long entityId, bool cacheFlag)
        {
            if (!isGetFromCache.ContainsKey(entityId))
            {
                isGetFromCache.Add(entityId, cacheFlag);
            }
            else
            {
                isGetFromCache[entityId] = cacheFlag;
            }
        }


        protected void DeleteCacheFlag(long entityId)
        {
            isGetFromCache.Remove(entityId);
        }


        protected bool IsFromCache(long entityId)
        {
            if (!isGetFromCache.ContainsKey(entityId))
            {
                isGetFromCache.Add(entityId, false);
            }

            return isGetFromCache[entityId];
        }
    }
}
