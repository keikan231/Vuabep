namespace CRS.Web.Models.FileManagement
{
    public interface IFileManager
    {
        void DeleteImagePair(string pairUrl);
        void DeleteSingleThumbnailImage(string part);
    }
}