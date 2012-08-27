using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.Common.Caching
{
    /// <summary>
    /// Indicates an object that can be cached
    /// </summary>
    public interface ICacheItem
    {
        /// <summary>
        /// Populates data for this cache item
        /// </summary>
        void Execute();

        /// <summary>
        /// Checks whether this object has been successfully populated
        /// </summary>
        bool IsPopulated { get; }
    }
}