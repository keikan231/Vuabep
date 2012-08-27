using CRS.Business.Models.Caching;
using CRS.Common;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
    /// <summary>
    /// Contains properties to access cached reference data
    /// </summary>
    public static class ReferenceDataCache
    {
        public static RoleCollection RoleCollection
        {
            get { return Cacher.GetData<RoleCollection>(Constants.ConfigKeys.Caching.RoleCollectionCacheMinutes); }
        }

        public static TipCategoryCollection TipCategoryCollection
        {
            get { return Cacher.GetData<TipCategoryCollection>(Constants.ConfigKeys.Caching.TipCategoryCollectionCacheMinutes); }
        }

        public static RecipeCategoryCollection RecipeCategoryCollection
        {
            get { return Cacher.GetData<RecipeCategoryCollection>(Constants.ConfigKeys.Caching.RecipeCategoryCollectionCacheMinutes); }
        }

        public static RecipeAllCategoryCollection RecipeAllCategoryCollection
        {
            get { return Cacher.GetData<RecipeAllCategoryCollection>(Constants.ConfigKeys.Caching.RecipeAllCategoryCollectionCacheMinutes); }
        }

        public static RecipeSmallCategoryCollection RecipeSmallCategoryCollection
        {
            get { return Cacher.GetData<RecipeSmallCategoryCollection>(Constants.ConfigKeys.Caching.RecipeSmallCategoryCollectionCacheMinutes); }
        }

        public static TagCategoryCollection TagCategoryCollection
        {
            get { return Cacher.GetData<TagCategoryCollection>(Constants.ConfigKeys.Caching.TagCategoryCollectionCacheMinutes); }
        }

        public static PermissionCollection PermissionCollection
        {
            get { return Cacher.GetData<PermissionCollection>(Constants.ConfigKeys.Caching.PermissionCollectionCacheMinutes); }
        }

        public static UserStateCollection UserStateCollection
        {
            get { return Cacher.GetData<UserStateCollection>(Constants.ConfigKeys.Caching.UserStateCollectionCacheMinutes); }
        }

        public static PointConfigCollection PointConfigCollection
        {
            get { return Cacher.GetData<PointConfigCollection>(Constants.ConfigKeys.Caching.PointConfigCollectionCacheMinutes); }
        }

        public static TitleCollection TitleCollection
        {
            get { return Cacher.GetData<TitleCollection>(Constants.ConfigKeys.Caching.TitleCollectionCacheMinutes); }
        }

        public static LocationCollection LocationCollection
        {
            get { return Cacher.GetData<LocationCollection>(Constants.ConfigKeys.Caching.LocationCollectionCacheMinutes); }
        }
    }
}