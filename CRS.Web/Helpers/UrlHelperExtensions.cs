using System;
using System.Web.Mvc;
using CRS.Business.Models.Entities;
using CRS.Common;
using CRS.Common.Helpers;
using CRS.Web.Models;

namespace CRS.Web.Helpers
{
    public static class UrlHelperExtensions
    {
        public static string NewsLink(this UrlHelper url, int id, string titleUrl)
        {
            return url.Action("Details", "News", new { area = (string)null, newsTitleUrl = titleUrl, id = id });
        }

        public static string RecipeLink(this UrlHelper url, int id, string titleUrl)
        {
            return url.Action("Details", "Recipes", new { area = (string)null, recipeTitleUrl = titleUrl, id = id });
        }

        public static string UserLink(this UrlHelper url, int id, string username)
        {
            return url.Action("Details", "Account", new { area = (string)null, username = username.ToUrlFriendly(), id = id });
        }

        public static string ShowImage(this UrlHelper url, string image, ImageType type)
        {
            var parts = image.Split(Constants.ImageUrlsSeparator);
            string imageUrl;
            switch (type)
            {
                case ImageType.Thumbnail:
                    imageUrl = parts[0];
                    break;
                case ImageType.Big:
                    imageUrl = parts[1];
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            return url.Content(imageUrl);
        }

        public static string UserAvatar(this UrlHelper url, User user, ImageType type)
        {
            string imageUrl;
            if (user.AvatarUrl != null)
            {
                var parts = user.AvatarUrl.Split(Constants.ImageUrlsSeparator);
                switch (type)
                {
                    case ImageType.Thumbnail:
                        imageUrl = parts[0];
                        break;
                    case ImageType.Big:
                        imageUrl = parts[1];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("type");
                }
            }
            else
            {
                imageUrl = AppConfigs.AvatarFolderPath + Constants.AnonymousAvatar;
            }

            return url.Content(imageUrl);
        }

        public static string UserAvatarUrl(this UrlHelper url, string avatarUrl, ImageType type)
        {
            string imageUrl;
            if (avatarUrl != null)
            {
                var parts = avatarUrl.Split(Constants.ImageUrlsSeparator);
                switch (type)
                {
                    case ImageType.Thumbnail:
                        imageUrl = parts[0];
                        break;
                    case ImageType.Big:
                        imageUrl = parts[1];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("type");
                }
            }
            else
            {
                imageUrl = AppConfigs.AvatarFolderPath + Constants.AnonymousAvatar;
            }

            return url.Content(imageUrl);
        }

        public static string QuestionLink(this UrlHelper url, int id, string titleUrl)
        {
            return url.Action("Details", "Questions", new { area = (string)null, questionTitleUrl = titleUrl, id = id });
        }

        public static string TipLink(this UrlHelper url, int id, string titleUrl)
        {
            return url.Action("Details", "Tips", new { area = (string)null, tipTitleUrl = titleUrl, id = id });
        }
    }
}