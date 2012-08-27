using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;

namespace CRS.Web.Framework
{
    /// <summary>
    /// DependencyResolver that uses Unity as DI container
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver
    {
        private IUnityContainer _container;

        public UnityDependencyResolver(IUnityContainer container)
        {
            _container = container;
        }

        #region Implementation of IDependencyResolver

        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch (Exception) // Return null for the framework to perform the default implementation
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return _container.ResolveAll(serviceType);
            }
            catch (Exception)
            {
                return new List<object>();
            }
        }

        #endregion
    }
}