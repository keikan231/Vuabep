using System.Web;
using CRS.Business.Feedbacks;

namespace CRS.Web.Models.FileManagement
{
    public interface IUploadHandler
    {
        Feedback<string> UploadImage(HttpPostedFileBase file);
        Feedback<string[]> UploadRecipeImage(HttpPostedFileBase file);
        Feedback<string[]> UploadUserAvatar(int userId, HttpPostedFileBase file);
    }
}