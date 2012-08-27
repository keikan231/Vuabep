using System.Threading;
using CRS.Business.Interfaces;
using CRS.Common;
using Microsoft.Practices.Unity;

namespace CRS.Web.Models
{
    public class PageViewManager
    {
        private static int _onlineVisitors;

        public static int GetOnlineVisitorNumber()
        {
            return _onlineVisitors;
        }

        public static void IncreaseVisitorNumber()
        {
            var _repository = IoC.UnityContainer.Resolve<IApplicationRepository>();
            Interlocked.Increment(ref _onlineVisitors);
            _repository.IncreasePageView();
        }

        public static void DecreaseVisitorNumber()
        {
            Interlocked.Decrement(ref _onlineVisitors);
        }
    }
}