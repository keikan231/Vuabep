using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.Common.Caching
{
    public static class CacheHelper
    {
        public static DateTime GetAbsoluteExpirationFromConfig(string key)
        {
            return DateTime.Now.AddMinutes(AppConfigs.GetCacheMinutes(key));
        }
    }
}