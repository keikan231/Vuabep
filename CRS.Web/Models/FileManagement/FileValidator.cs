using System.IO;
using CRS.Common;
using System.Linq;

namespace CRS.Web.Models.FileManagement
{
    public static class FileValidator
    {
        public static bool IsValidImageExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                return false;

            return Constants.AllowedImageExtensions.Contains(extension.ToLower());
        }
    }
}