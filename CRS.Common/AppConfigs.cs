using System;
using System.Configuration;

namespace CRS.Common
{
    /// <summary>
    /// Contains properties to get corresponding configurations in config files
    /// </summary>
    public class AppConfigs
    {
        public static string GetSetting(string key)
        {
            string appSetting = ConfigurationManager.AppSettings[key];
            if (appSetting == null)
            {
                throw new ArgumentOutOfRangeException("key", string.Format("Cannot find setting with key {0}", key));
            }

            return appSetting;
        }

        public static int GetIntSetting(string key)
        {
            return int.Parse(GetSetting(key));
        }

        public static double GetDoubleSetting(string key)
        {
            return double.Parse(GetSetting(key));
        }

        public static int LoginAttemptsAllowed
        {
            get { return GetIntSetting(Constants.ConfigKeys.LoginAttemptsAllowed); }
        }

        public static int LoginAttemptLockMinutes
        {
            get { return GetIntSetting(Constants.ConfigKeys.LoginAttemptLockMinutes); }
        }

        public static string SiteEmail
        {
            get { return GetSetting(Constants.ConfigKeys.SiteEmail); }
        }

        /// <summary>
        /// Number of minutes to cache an item with a specific key
        /// </summary>
        public static int GetCacheMinutes(string key)
        {
            return GetIntSetting(key);
        }

        public static int NewsPageSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.NewsPageSize); }
        }

        public static int TipPageSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.TipPageSize); }
        }

        public static int AllPageSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.AllPageSize); }
        }

        public static int UploadImageMaxSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.UploadImageMaxSize); }
        }

        public static string ThumbnailImageMaxSize
        {
            get { return GetSetting(Constants.ConfigKeys.ThumbnailImageMaxSize); }
        }

        public static string BigImageMaxSize
        {
            get { return GetSetting(Constants.ConfigKeys.BigImageMaxSize); }
        }

        public static int ImageMinSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.ImageMinSize); }
        }

        public static string ImagePath
        {
            get { return GetSetting(Constants.ConfigKeys.ImagePath); }
        }

        public static string AvatarFolderPath
        {
            get { return GetSetting(Constants.ConfigKeys.AvatarFolderPath); }
        }

        public static string UserAvatarThumbnailImageMaxSize
        {
            get { return GetSetting(Constants.ConfigKeys.UserAvatarThumbnailImageMaxSize); }
        }

        public static string UserAvatarBigImageMaxSize
        {
            get { return GetSetting(Constants.ConfigKeys.UserAvatarBigImageMaxSize); }
        }

        public static int QuestionsPageSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.QuestionsPageSize); }
        }

        public static int AnswersPageSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.AnswersPageSize); }
        }

        public static int HighlightAnswerNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.HighlightAnswerNumber); }
        }

        public static int HighlightCommentNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.HighlightCommentNumber); }
        }

        public static int CommentsPageSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.CommentsPageSize); }
        }

        public static int RecipePageSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.RecipePageSize); }
        }

        public static int DefaultAdminGridPageSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.DefaultAdminGridPageSize); }
        }

        public static int MinReportNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.MinReportNumber); }
        }

        public static int TagCloudSize
        {
            get { return GetIntSetting(Constants.ConfigKeys.TagCloudSize); }
        }

        public static string AdminEmail
        {
            get { return GetSetting(Constants.ConfigKeys.AdminEmail); }
        }

        public static int HotQuestionsInMonthNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.HotQuestionsInMonthNumber); }
        }

        public static int TopNewsNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.TopNewsNumber); }
        }

        public static int TopTipNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.TopTipNumber); }
        }

        public static int TopTipByCategoryNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.TopTipByCategoryNumber); }
        }

        public static int TopMappedTipByCategoryNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.TopMappedTipByCategoryNumber); }
        }

        public static int TopRecipeNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.TopRecipeNumber); }
        }

        public static int TopRecipeByCategoryNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.TopRecipeByCategoryNumber); }
        }

        public static int TopAnswerContributorNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.TopAnswerContributorNumber); }
        }

        public static int TopContributorNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.TopContributorNumber); }
        }

        public static int NewQuestionNumber
        {
            get { return GetIntSetting(Constants.ConfigKeys.NewQuestionNumber); }
        }
    }
}