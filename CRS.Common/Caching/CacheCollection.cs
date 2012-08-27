using System.Collections.Generic;

namespace CRS.Common.Caching
{
    public abstract class CacheCollection<T> : List<T>, ICacheItem
    {
        #region Implementation of ICacheItem

        public abstract void Execute();

        public bool IsPopulated { get; protected set; }

        #endregion
    }
}