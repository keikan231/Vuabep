using System;
using System.IO;
using System.Web;
using CRS.Common;
using CRS.Common.Logging;

namespace CRS.Web.Models.FileManagement
{
    public class FileManager : IFileManager
    {
        #region Implementation of IFileManager

        public void DeleteImagePair(string pairUrl)
        {
            try
            {
                var parts = pairUrl.Split(Constants.ImageUrlsSeparator);
                foreach (var part in parts)
                {
                    var physicalFile = HttpContext.Current.Server.MapPath(part);
                    File.Delete(physicalFile);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void DeleteSingleThumbnailImage(string part)
        {
            try
            {
                var physicalFile = HttpContext.Current.Server.MapPath(part);
                File.Delete(physicalFile);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        #endregion
    }
}