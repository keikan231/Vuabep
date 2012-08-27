using Microsoft.Practices.Unity;
using CRS.Web.Models.FileManagement;

namespace CRS.Web.Configuration
{
    /// <summary>
    /// Includes configuration for CRS.Web assembly to be used with Unity Container
    /// </summary>
    public class WebUnityExtension : UnityContainerExtension
    {
        #region Overrides of UnityContainerExtension

        protected override void Initialize()
        {
            Container
                //.RegisterType<ISearchHandler, SearchHandler>() // TODO - KIENTT - Check later
                .RegisterType<IUploadHandler, UploadHandler>()
                .RegisterType<IFileManager, FileManager>();
        }

        #endregion
    }
}