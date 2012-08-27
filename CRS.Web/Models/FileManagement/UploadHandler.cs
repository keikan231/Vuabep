using System;
using System.Drawing;
using System.IO;
using System.Web;
using CRS.Business.Feedbacks;
using CRS.Common;
using CRS.Common.ImageProcessing;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Web.Models.FileManagement
{
    public class UploadHandler : IUploadHandler
    {
        #region Implementation of IUploadHandler

        public Feedback<string> UploadImage(HttpPostedFileBase file)
        {
            if (!FileValidator.IsValidImageExtension(file.FileName))
                return new Feedback<string>(false, Messages.ImageExtensionNotAllowed);
            if (file.ContentLength > AppConfigs.UploadImageMaxSize * 1024 * 1024)
                return new Feedback<string>(false, Messages.ExceedMaxContentLength);

            try
            {
                Image image = Image.FromStream(file.InputStream);
                // Check for min image dimensions
                if (image.Width < AppConfigs.ImageMinSize || image.Height < AppConfigs.ImageMinSize)
                    return new Feedback<string>(false, Messages.ImageTooSmall);

                string contentFolder = HttpContext.Current.Server.MapPath(AppConfigs.ImagePath);

                // IE pass file name with full path
                string fileName = Path.GetFileName(file.FileName);
                // Get file names to save. All images should be saved as jpg.
                fileName = GetSafeFileName(fileName);
                int lastDot = fileName.LastIndexOf(".");
                string thumbnailFileName = string.Format("{0}_small.jpg", fileName.Substring(0, lastDot));
                thumbnailFileName = GetUniqueName(thumbnailFileName, contentFolder);
                string bigFileName = string.Format("{0}.jpg", fileName.Substring(0, lastDot));
                bigFileName = GetUniqueName(bigFileName, contentFolder);

                // Resizer object to perform image processing
                IImageResizer imageResizer = new JpegImageResizer(image);
                // Create thumbnail image
                string[] thumb = AppConfigs.ThumbnailImageMaxSize.Split('x');
                imageResizer.ScaleToFit(int.Parse(thumb[0]), int.Parse(thumb[1]));
                imageResizer.SaveToFile(string.Format("{0}\\{1}", contentFolder, thumbnailFileName));
                // Create big image
                //string[] big = AppConfigs.BigImageMaxSize.Split('x');
                //imageResizer.ScaleToFit(int.Parse(big[0]), int.Parse(big[1]));
                //imageResizer.SaveToFile(string.Format("{0}\\{1}", placeFolder, bigFileName));

                return new Feedback<string>(true, null, string.Format("{0}/{1}", AppConfigs.ImagePath, thumbnailFileName)
                                                                  //string.Format("{0}/{1}", AppConfigs.ImagePath, bigFileName)
                                                              );
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<string>(false, Messages.GeneralError);
            }
        }

        public Feedback<string[]> UploadRecipeImage(HttpPostedFileBase file)
        {
            if (!FileValidator.IsValidImageExtension(file.FileName))
                return new Feedback<string[]>(false, Messages.ImageExtensionNotAllowed);
            if (file.ContentLength > AppConfigs.UploadImageMaxSize * 1024 * 1024)
                return new Feedback<string[]>(false, Messages.ExceedMaxContentLength);

            try
            {
                Image image = Image.FromStream(file.InputStream);

                // Get folder to save
                string relativeContentFolder = AppConfigs.ImagePath;
                string contentFolder = HttpContext.Current.Server.MapPath(relativeContentFolder);

                // IE pass file name with full path
                string fileName = Path.GetFileName(file.FileName);
                // Get file names to save. All images should be saved as jpg.
                fileName = GetSafeFileName(fileName);
                int lastDot = fileName.LastIndexOf(".");
                string thumbnailFileName = string.Format("{0}_small.jpg", fileName.Substring(0, lastDot));
                thumbnailFileName = GetUniqueName(thumbnailFileName, contentFolder);
                string bigFileName = string.Format("{0}.jpg", fileName.Substring(0, lastDot));
                bigFileName = GetUniqueName(bigFileName, contentFolder);

                // Resizer object to perform image processing
                IImageResizer imageResizer = new JpegImageResizer(image);
                // Create thumbnail image
                string[] thumb = AppConfigs.ThumbnailImageMaxSize.Split('x');
                imageResizer.ScaleToFit(int.Parse(thumb[0]), int.Parse(thumb[1]));
                imageResizer.SaveToFile(string.Format("{0}\\{1}", contentFolder, thumbnailFileName));
                // Create big image
                string[] big = AppConfigs.BigImageMaxSize.Split('x');
                imageResizer.ScaleToFit(int.Parse(big[0]), int.Parse(big[1]));
                imageResizer.SaveToFile(string.Format("{0}\\{1}", contentFolder, bigFileName));

                return new Feedback<string[]>(true, null, new[]
                                                              {
                                                                  string.Format("{0}{1}", relativeContentFolder, thumbnailFileName),
                                                                  string.Format("{0}{1}", relativeContentFolder, bigFileName)
                                                              });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<string[]>(false, Messages.GeneralError);
            }
        }

        public Feedback<string[]> UploadUserAvatar(int userId, HttpPostedFileBase file)
        {
            if (!FileValidator.IsValidImageExtension(file.FileName))
                return new Feedback<string[]>(false, Messages.ImageExtensionNotAllowed);
            if (file.ContentLength > AppConfigs.UploadImageMaxSize * 1024 * 1024)
                return new Feedback<string[]>(false, Messages.ExceedMaxContentLength);

            try
            {
                Image image = Image.FromStream(file.InputStream);

                // Get folder to save
                string relativeAvatarFolder = AppConfigs.AvatarFolderPath;
                string avatarFolder = HttpContext.Current.Server.MapPath(relativeAvatarFolder);

                // IE pass file name with full path
                string fileName = Path.GetFileName(file.FileName);
                // Get file names to save. All images should be saved as jpg.
                fileName = GetSafeFileName(fileName);
                int lastDot = fileName.LastIndexOf(".");
                string thumbnailFileName = string.Format("{0}_small.jpg", fileName.Substring(0, lastDot));
                thumbnailFileName = GetUniqueName(thumbnailFileName, avatarFolder);
                string bigFileName = string.Format("{0}.jpg", fileName.Substring(0, lastDot));
                bigFileName = GetUniqueName(bigFileName, avatarFolder);

                // Resizer object to perform image processing
                IImageResizer imageResizer = new JpegImageResizer(image);
                // Create thumbnail image
                string[] thumb = AppConfigs.UserAvatarThumbnailImageMaxSize.Split('x');
                imageResizer.ScaleToFit(int.Parse(thumb[0]), int.Parse(thumb[1]));
                imageResizer.SaveToFile(string.Format("{0}\\{1}", avatarFolder, thumbnailFileName));
                // Create big image
                string[] big = AppConfigs.UserAvatarBigImageMaxSize.Split('x');
                imageResizer.ScaleToFit(int.Parse(big[0]), int.Parse(big[1]));
                imageResizer.SaveToFile(string.Format("{0}\\{1}", avatarFolder, bigFileName));

                return new Feedback<string[]>(true, null, new[]
                                                              {
                                                                  string.Format("{0}{1}", relativeAvatarFolder, thumbnailFileName),
                                                                  string.Format("{0}{1}", relativeAvatarFolder, bigFileName)
                                                              });
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<string[]>(false, Messages.GeneralError);
            }
        }

        #endregion

        #region Private methods

        private string GetSafeFileName(string fileName)
        {
            return fileName.Replace("%", "").Replace(Constants.ImageUrlsSeparator.ToString(), "");
        }

        private string GetUniqueName(string fileName, string folder)
        {
            string ret = fileName;
            int i = 1;
            while (File.Exists(string.Format("{0}\\{1}", folder, ret)))
            {
                i++;
                int lastDot = fileName.LastIndexOf(".");
                ret = string.Format("{0} ({1}){2}", fileName.Substring(0, lastDot), i, fileName.Substring(lastDot));
            }

            return ret;
        }

        #endregion

    }
}