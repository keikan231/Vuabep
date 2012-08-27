using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CRS.Business.Configuration;
using CRS.Common;
using CRS.Common.Logging;
using CRS.Web.Configuration;
using CRS.Web.Framework;
using CRS.Web.Framework.ModelBinders;
using CRS.Web.Framework.Routing;
using CRS.Web.Framework.Security;
using CRS.Web.Models;
using Microsoft.Practices.Unity;

namespace CRS.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // News list page
            routes.MapRoute("News", "tin-tuc",
                            new { controller = "News", action = "Index" });

            // News details page
            routes.MapRoute("NewsDetails", "tin-tuc/{newsTitleUrl}-{id}",
                            new { controller = "News", action = "Details" });

            // Tip list page
            routes.MapRoute("Tips", "meo-vat",
                            new { controller = "Tips", action = "Index" });

            // Recipe list page
            routes.MapRoute("Recipes", "cong-thuc",
                            new { controller = "Recipes", action = "Index" });

            // Tip details page
            routes.MapRoute("TipDetails", "meo-vat/{tipTitleUrl}-{id}",
                            new { controller = "Tips", action = "Details" });

            // Recipe details page
            routes.MapRoute("RecipeDetails", "cong-thuc/{recipeTitleUrl}-{id}",
                            new { controller = "Recipes", action = "Details" });

            // Tip category page
            routes.MapRoute("TipCategories", "meo-vat/chuyen-muc/{tipCategoryNameUrl}-{id}",
                            new { controller = "Tips", action = "TipCategoryIndex" });

            // Recipe category page
            routes.MapRoute("RecipeCategories", "cong-thuc/chuyen-muc/{recipeCategoryNameUrl}-{id}",
                            new { controller = "Recipes", action = "RecipeCategoryIndex" });

            // Recipe small category page
            routes.MapRoute("RecipeSmallCategories", "cong-thuc/chuyen-muc-nho/{recipeCategoryNameUrl}-{id}-{smallId}",
                            new { controller = "Recipes", action = "RecipeSmallCategoryIndex" });

            // Question list page
            routes.MapRoute("Questions", "hoi-dap",
                            new { controller = "Questions", action = "Index" });

            // User details page
            routes.MapRoute("UserDetails", "Account/{username}-{id}",
                            new { controller = "Account", action = "Details" });

            // Question details page
            routes.MapRoute("QuestionDetails", "hoi-dap/{questionTitleUrl}-{id}",
                            new { controller = "Questions", action = "Details" });
            
            //Display http404 error
            routes.MapRoute("http404", "Error",
                            new { controller = "Error", action = "Index" }
);

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        public static void RegisterDependencyInjection()
        {
            // Setup the dependency container
            IUnityContainer container = new UnityContainer()
                .AddNewExtension<BusinessUnityExtension>()
                .AddNewExtension<WebUnityExtension>();

            // This container will be used throughout the application lifetime
            IoC.UnityContainer = container;

            // And set as MVC dependency resolver
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        protected void Application_Start()
        {
            Logger.Info("Application starts!");
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterDependencyInjection();
            ModelBinders.Binders.DefaultBinder = new TrimModelBinder();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Logger.Info("Application shuts down!");
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            PageViewManager.IncreaseVisitorNumber();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            PageViewManager.DecreaseVisitorNumber();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (!(ex is HttpException))
                Logger.Error(ex);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                // Try replacing the default Principal by PermissionPrincipal
                var principal = SecurityCacheManager.GetPrincipal(User.Identity.Name);
                if (principal != null)
                {
                    HttpContext.Current.User = principal;
                }
                else // If for any reason, the user is accidentally logged in, cancel the request
                {
                    FormsAuthentication.SignOut();
                    Response.Redirect("~");
                }
            }
        }
    }
}