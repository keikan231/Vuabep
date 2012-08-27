using System;
using System.Collections.Generic;
using CRS.Business.Models;

namespace CRS.Web.Areas.Admin.Models
{
    /// <summary>
    /// Holds info of all main menu items of admin pages
    /// </summary>
    public static class AdminMenuCollection
    {
        public static IList<AdminMenuParent> Items { get; private set; }

        static AdminMenuCollection()
        {
            Items = new List<AdminMenuParent>();
            CreateContentsMenus();
            CreateSpamMenus();
            CreateUserMenus();
            CreateCategoryMenus();
            CreateApproveMenus();
            CreateDictionaryMenus();
            CreateStatMenus();
#if DEBUG
            Validate();
#endif
        }
#if DEBUG
        private static void Validate()
        {
            IList<string> parents = new List<string>();
            foreach (var parent in Items)
            {
                // Check to ensure all menu groups have a unique name
                if (parents.Contains(parent.Name))
                    throw new InvalidOperationException(string.Format("2 menus have the same name: {0}", parent.Name));
                parents.Add(parent.Name);

                // Check to ensure all child menus inside a group have a unique name
                IList<string> children = new List<string>();
                foreach (var child in parent.Children)
                {
                    if (children.Contains(child.Name))
                        throw new InvalidOperationException(string.Format("2 menus have the same name: {0}", child.Name));
                    children.Add(child.Name);
                }
            }
        }
#endif

        private static void CreateApproveMenus()
        {
            IList<AdminMenuChild> children = new List<AdminMenuChild>();
            Items.Add(new AdminMenuParent("Approval", "Approval", children));
            children.Add(new AdminMenuChild("ApproveRecipes", "Approve Recipes", "Index", "ManageApproval", KeyObject.Permission.ManageApproval));
            children.Add(new AdminMenuChild("UnapproveRecipes", "Unapprove Recipes", "ApprovedRecipesIndex", "ManageApproval", KeyObject.Permission.ManageApproval));
        }

        private static void CreateDictionaryMenus()
        {
            IList<AdminMenuChild> children = new List<AdminMenuChild>();
            Items.Add(new AdminMenuParent("Dictionary", "Dictionary", children));
            children.Add(new AdminMenuChild("ApproveWords", "Approve Words", "Index", "ManageDictionary", KeyObject.Permission.ManageDictionary));
            children.Add(new AdminMenuChild("UnapproveWords", "Unapprove Words", "ApprovedWordIndex", "ManageDictionary", KeyObject.Permission.ManageDictionary));
            children.Add(new AdminMenuChild("ApproveUpdatedWords", "Approve Updated Words", "UnapprovedUpdatedWordIndex", "ManageDictionary", KeyObject.Permission.ManageDictionary));
        }

        private static void CreateCategoryMenus()
        {
            IList<AdminMenuChild> children = new List<AdminMenuChild>();
            Items.Add(new AdminMenuParent("Categories", "Categories", children));
            children.Add(new AdminMenuChild("ManageTipCategories", "Manage Tip Categories", "ManageTipCategories", "ManageCategories", KeyObject.Permission.ManageCategories));
            children.Add(new AdminMenuChild("CreateTipCategory", "Create a new Tip Category", "CreateTipCategory", "ManageCategories", KeyObject.Permission.ManageCategories));
            children.Add(new AdminMenuChild("ManageRecipeCategories", "Manage Recipe Categories", "ManageRecipeCategories", "ManageCategories", KeyObject.Permission.ManageCategories));
            children.Add(new AdminMenuChild("CreateRecipeCategory", "Create a new Recipe Category", "CreateRecipeCategory", "ManageCategories", KeyObject.Permission.ManageCategories));
            children.Add(new AdminMenuChild("ManageRecipeSmallCategories", "Manage Small Recipe Categories", "ManageRecipeSmallCategories", "ManageCategories", KeyObject.Permission.ManageCategories));
            children.Add(new AdminMenuChild("CreateRecipeSmallCategory", "Create a new Small Recipe Category", "CreateRecipeSmallCategory", "ManageCategories", KeyObject.Permission.ManageCategories));
        }

        private static void CreateContentsMenus()
        {
            IList<AdminMenuChild> children = new List<AdminMenuChild>();
            Items.Add(new AdminMenuParent("Contents", "Contents", children));
            children.Add(new AdminMenuChild("ManageNews", "Manage News", "ManageNews", "ManageContents", KeyObject.Permission.ManageContents));
            children.Add(new AdminMenuChild("ManageRecipes", "Manage Recipes", "ManageRecipes", "ManageContents", KeyObject.Permission.ManageContents));
            children.Add(new AdminMenuChild("ManageTips", "Manage Tips", "ManageTips", "ManageContents", KeyObject.Permission.ManageContents));
            children.Add(new AdminMenuChild("ManageQuestions", "Manage Questions", "ManageQuestions", "ManageContents", KeyObject.Permission.ManageContents));
        }

        private static void CreateSpamMenus()
        {
            IList<AdminMenuChild> children = new List<AdminMenuChild>();
            Items.Add(new AdminMenuParent("Spam", "Spam", children));
            children.Add(new AdminMenuChild("ManageReportedNews", "Reported News", "ManageReportedNews", "ManageSpam", KeyObject.Permission.ManageSpam));
            children.Add(new AdminMenuChild("ManageReportedRecipes", "Reported Recipes", "ManageReportedRecipe", "ManageSpam", KeyObject.Permission.ManageSpam));
            children.Add(new AdminMenuChild("ManageReportedTips", "Reported Tips", "ManageReportedTip", "ManageSpam", KeyObject.Permission.ManageSpam));
            children.Add(new AdminMenuChild("ManageReportedQuestions", "Reported Questions", "ManageReportedQuestion", "ManageSpam", KeyObject.Permission.ManageSpam));
            children.Add(new AdminMenuChild("ManageReportedAnswers", "Reported Answers", "ManageReportedAnswer", "ManageSpam", KeyObject.Permission.ManageSpam));
            children.Add(new AdminMenuChild("ManageReportedNewsComments", "Reported News Comments", "ManageReportedNewsComment", "ManageSpam", KeyObject.Permission.ManageSpam));
            children.Add(new AdminMenuChild("ManageReportedRecipeComments", "Reported Recipe Comments", "ManageReportedRecipeComment", "ManageSpam", KeyObject.Permission.ManageSpam));
            children.Add(new AdminMenuChild("ManageReportedTipComments", "Reported Tip Comments", "ManageReportedTipComment", "ManageSpam", KeyObject.Permission.ManageSpam));
        }

        private static void CreateUserMenus()
        {
            IList<AdminMenuChild> children = new List<AdminMenuChild>();
            Items.Add(new AdminMenuParent("Users", "Users", children));
            children.Add(new AdminMenuChild("ManageUsers", "Manage Users", "Index", "ManageUsers", KeyObject.Permission.ManageUsers));
            children.Add(new AdminMenuChild("ManageRoles", "Manage Roles", "Index", "ManageRoles", KeyObject.Permission.ManageUsers));
            children.Add(new AdminMenuChild("CreateRole", "Create a new Role", "Create", "ManageRoles", KeyObject.Permission.ManageUsers));
        }

        private static void CreateStatMenus()
        {
            IList<AdminMenuChild> children = new List<AdminMenuChild>();
            Items.Add(new AdminMenuParent("Statistics", "Statistics", children));
            children.Add(new AdminMenuChild("ViewStatistics", "Website Statistics", "Index", "AdminStatistics", KeyObject.Permission.ViewStatistics));
        }
    }
}
