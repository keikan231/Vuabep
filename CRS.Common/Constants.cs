namespace CRS.Common
{
    public class Constants
    {
        public static class ConfigKeys
        {
            public static class Caching
            {
                public const string PrincipalCacheMinutes = "PrincipalCacheMinutes";
                public const string RoleCollectionCacheMinutes = "RoleCollectionCacheMinutes";
                public const string TipCategoryCollectionCacheMinutes = "TipCategoryCollectionCacheMinutes";
                public const string TagCategoryCollectionCacheMinutes = "TagCategoryCollectionCacheMinutes";
                public const string AddressPartCollectionCacheMinutes = "AddressPartCollectionCacheMinutes";
                public const string PermissionCollectionCacheMinutes = "PermissionCollectionCacheMinutes";
                public const string AddressPartPrefixCollectionCacheMinutes = "AddressPartPrefixCollectionCacheMinutes";
                public const string FeaturedPlaceCollectionCacheMinutes = "FeaturedPlaceCollectionCacheMinutes";
                public const string UserStateCollectionCacheMinutes = "UserStateCollectionCacheMinutes";
                public const string RevisionStateCollectionCacheMinutes = "RevisionStateCollectionCacheMinutes";
                public const string PointConfigCollectionCacheMinutes = "PointConfigCollectionCacheMinutes";
                public const string TitleCollectionCacheMinutes = "TitleCollectionCacheMinutes";
                public const string LocationCollectionCacheMinutes = "LocationCollectionCacheMinutes";
                public const string RecipeSmallCategoryCollectionCacheMinutes = "RecipeSmallCategoryCollectionCacheMinutes";
                public const string RecipeCategoryCollectionCacheMinutes = "RecipeCategoryCollectionCacheMinutes";
                public const string RecipeAllCategoryCollectionCacheMinutes = "RecipeAllCategoryCollectionCacheMinutes";
            }
            public const string TopMappedTipByCategoryNumber = "TopMappedTipByCategoryNumber";
            public const string TopRecipeByCategoryNumber = "TopRecipeByCategoryNumber";
            public const string TopRecipeNumber = "TopRecipeNumber";
            public const string RecipePageSize = "RecipePageSize";
            public const string TopContributorNumber = "TopContributorNumber";
            public const string NewQuestionNumber = "NewQuestionNumber";
            public const string TopTipByCategoryNumber = "TopTipByCategoryNumber";
            public const string TopAnswerContributorNumber ="TopAnswerContributorNumber";
            public const string AllPageSize = "AllPageSize";
            public const string TopTipNumber = "TopTipNumber";
            public const string TopNewsNumber = "TopNewsNumber";
            public const string HotQuestionsInMonthNumber = "HotQuestionsInMonthNumber";
            public const string AdminEmail = "AdminEmail";
            public const string TagCloudSize = "TagCloudSize";
            public const string MinReportNumber = "MinReportNumber";
            public const string DefaultAdminGridPageSize = "DefaultAdminGridPageSize";
            public const string HighlightCommentNumber = "HighlightCommentNumber";
            public const string CommentsPageSize = "CommentsPageSize";
            public const string HighlightAnswerNumber = "HighlightAnswerNumber";
            public const string QuestionsPageSize = "QuestionsPageSize";
            public const string AnswersPageSize = "AnswersPageSize";
            public const string UserAvatarThumbnailImageMaxSize = "UserAvatarThumbnailImageMaxSize";
            public const string UserAvatarBigImageMaxSize = "UserAvatarBigImageMaxSize";
            public const string AvatarFolderPath = "AvatarFolderPath";
            public const string ImagePath = "ImagePath";
            public const string ThumbnailImageMaxSize = "ThumbnailImageMaxSize";
            public const string BigImageMaxSize = "BigImageMaxSize";
            public const string ImageMinSize = "ImageMinSize";
            public const string UploadImageMaxSize = "UploadImageMaxSize";
            public const string LoginAttemptsAllowed = "LoginAttemptsAllowed";
            public const string LoginAttemptLockMinutes = "LoginAttemptLockMinutes";
            public const string SiteEmail = "SiteEmail";
            public const string TipPageSize = "TipPageSize";
            public const string NewsPageSize = "NewsPageSize";
        }

        public const string TitleHeading = "Vuabep.vn";
        public const string DescriptionTailing = "Mạng chia sẻ công thức nấu nướng Vuabep.vn";
        public const string AnonymousAvatar = "anonymous.png";
        public const char ImageUrlsSeparator = '#';
        public const string EncryptionType = "SHA1";
        public static readonly string[] AllowedImageExtensions = { ".jpg", ".png", ".jpeg" };
        public static readonly string[] AllowedHtmlTags = {
                                                              "div", "ul", "li", "ol", "a", "u", "b", "strong", "i", "em", "br", "font", "p", "span", "#text", "iframe", "img"
                                                          };

        public static readonly string[] AllowedHtmlAttributes = { "class", "style", "href", "frameborder", "height", "src", "title", "type", "width", "alt"};
    }
}