using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CRS.Common.Helpers;

namespace CRS.Web.Helpers
{

    public static class HtmlHelperExtensions
    {

        #region DropDownList for optiongroup

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name)
        {
            return DropDownList(htmlHelper, name, null /* selectList */, null /* optionLabel */, null /* htmlAttributes */ );
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, string optionLabel)
        {
            return DropDownList(htmlHelper, name, null /* selectList */, optionLabel, null /* htmlAttributes */ );
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IDictionary<string, IEnumerable<SelectListItem>> selectList)
        {
            return DropDownList(htmlHelper, name, selectList, null /* optionLabel */, null /* htmlAttributes */ );
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IDictionary<string, IEnumerable<SelectListItem>> selectList, object htmlAttributes)
        {
            return DropDownList(htmlHelper, name, selectList, null /* optionLabel */ , htmlAttributes);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IDictionary<string, IEnumerable<SelectListItem>> selectList, IDictionary<string, object> htmlAttributes)
        {
            return DropDownList(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IDictionary<string, IEnumerable<SelectListItem>> selectList, string optionLabel)
        {
            return DropDownList(htmlHelper, name, selectList, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IDictionary<string, IEnumerable<SelectListItem>> selectList, string optionLabel, object htmlAttributes)
        {
            return DropDownList(htmlHelper, name, selectList, optionLabel, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IDictionary<string, IEnumerable<SelectListItem>> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return DropDownListHelper(htmlHelper, name, selectList, optionLabel, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, IEnumerable<SelectListItem>> selectList)
        {
            return DropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, null /* htmlAttributes */ );
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, IEnumerable<SelectListItem>> selectList, object htmlAttributes, Expression<Func<TModel, TProperty>> childExpression, string url)
        {
            return DropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, new RouteValueDictionary(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, IEnumerable<SelectListItem>> selectList, string optionLabel, Expression<Func<TModel, TProperty>> childExpression, string childOptionLabel, string url)
        {
            return DropDownListFor(htmlHelper, expression, selectList, optionLabel, null /* htmlAttributes */);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, IEnumerable<SelectListItem>> selectList, object htmlAttributes)
        {
            return DropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, new RouteValueDictionary(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, IEnumerable<SelectListItem>> selectList, IDictionary<string, object> htmlAttributes)
        {
            return DropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, IEnumerable<SelectListItem>> selectList, string optionLabel)
        {
            return DropDownListFor(htmlHelper, expression, selectList, optionLabel, null /* htmlAttributes */);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, IEnumerable<SelectListItem>> selectList, string optionLabel, object htmlAttributes)
        {
            return DropDownListFor(htmlHelper, expression, selectList, optionLabel, new RouteValueDictionary(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, IEnumerable<SelectListItem>> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return DropDownListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList, optionLabel, htmlAttributes);
        }


        private static MvcHtmlString DropDownListHelper(HtmlHelper htmlHelper, string expression, IDictionary<string, IEnumerable<SelectListItem>> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return SelectInternal(htmlHelper, optionLabel, expression, selectList, false /* allowMultiple */, htmlAttributes);
        }
        #endregion

        #region Helper methods

        private static IDictionary<string, IEnumerable<SelectListItem>> GetSelectData(this HtmlHelper htmlHelper, string name)
        {
            object o = null;
            if (htmlHelper.ViewData != null)
            {
                o = htmlHelper.ViewData.Eval(name);
            }
            if (o == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "There is no ViewData item of type '{1}' that has the key '{0}'.", /*MvcResources.HtmlHelper_MissingSelectData*/
                        name,
                        "IDictionary<string , IEnumerable<SelectListItem>>"));
            }
            IDictionary<string, IEnumerable<SelectListItem>> selectList = o as IDictionary<string, IEnumerable<SelectListItem>>;
            if (selectList == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "The ViewData item that has the key '{0}' is of type '{1}' but must be of type '{2}'." /*MvcResources.HtmlHelper_WrongSelectDataType*/ ,
                        name,
                        o.GetType().FullName,
                        "IDictionary<string , IEnumerable<SelectListItem>>"));
            }
            return selectList;
        }

        internal static string ListItemToOption(string category, IEnumerable<SelectListItem> items)
        {
            TagBuilder builder = new TagBuilder("optgroup");
            builder.Attributes["label"] = category;

            // Convert each ListItem to an <option> tag
            StringBuilder listItemBuilder = new StringBuilder();

            foreach (SelectListItem item in items)
            {
                listItemBuilder.AppendLine(ListItemToOption(item));
            }

            builder.InnerHtml = listItemBuilder.ToString();

            return builder.ToString(TagRenderMode.Normal);
        }

        internal static string ListItemToOption(SelectListItem item)
        {
            TagBuilder builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            return builder.ToString(TagRenderMode.Normal);
        }

        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, string optionLabel, string name, IDictionary<string, IEnumerable<SelectListItem>> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("Value cannot be null or empty." /*MvcResources.Common_NullOrEmpty*/ , "name");
            }

            bool usedViewData = false;

            // If we got a null selectList, try to use ViewData to get the list of items.
            if (selectList == null)
            {
                selectList = htmlHelper.GetSelectData(fullName);
                usedViewData = true;
            }

            object defaultValue = (allowMultiple) ? GetModelStateValue(htmlHelper, fullName, typeof(string[])) : GetModelStateValue(htmlHelper, fullName, typeof(string));

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (!usedViewData)
            {
                if (defaultValue == null)
                {
                    defaultValue = htmlHelper.ViewData.Eval(fullName);
                }
            }

            // Convert each ListItem to an <option> tag
            StringBuilder listItemBuilder = new StringBuilder();

            if (defaultValue != null)
            {
                IEnumerable defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
                IEnumerable<string> values = from object value in defaultValues select Convert.ToString(value, CultureInfo.CurrentCulture);
                HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);

                Dictionary<string, IEnumerable<SelectListItem>> newSelectListDictionary = new Dictionary<string, IEnumerable<SelectListItem>>();
                foreach (var category in selectList.Keys)
                {
                    List<SelectListItem> newSelectList = new List<SelectListItem>();

                    foreach (SelectListItem item in selectList[category])
                    {
                        item.Selected = (item.Value != null) ?
                            selectedValues.Contains(item.Value) :
                            selectedValues.Contains(item.Text);
                        newSelectList.Add(item);
                    }

                    newSelectListDictionary.Add(category, newSelectList);
                }
                selectList = newSelectListDictionary;
            }

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
            {
                listItemBuilder.AppendLine(ListItemToOption(new SelectListItem() { Text = optionLabel, Value = String.Empty, Selected = false }));
            }

            foreach (var category in selectList.Keys)
            {
                listItemBuilder.AppendLine(ListItemToOption(category, selectList[category]));
            }

            TagBuilder tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };

            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
            tagBuilder.GenerateId(fullName);

            if (allowMultiple)
            {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        internal static object GetModelStateValue(HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }
        #endregion

        /*-----------------------------------------------------------------------------------*/

        public static MvcHtmlString NewsLink(this HtmlHelper html, int id, string title, string titleUrl, object htmlAttributes = null)
        {
            return html.ActionLink(title, "Details", "News", new { area = (string)null, newsTitleUrl = titleUrl, id = id }, htmlAttributes);
        }

        public static MvcHtmlString NewsImageActionLink(this HtmlHelper html, string controllerName, string imageUrl, string altText, int id, string title, string titleUrl, object imgAttributes = null, object linkAttributes = null)
        {
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", imageUrl);
            imgBuilder.MergeAttribute("alt", altText);
            imgBuilder.MergeAttributes(new RouteValueDictionary(imgAttributes), true);
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext, html.RouteCollection);
            var linkBuilder = new TagBuilder("a");
            linkBuilder.MergeAttribute("href", urlHelper.Action("Details", controllerName, new { area = (string)null, newsTitleUrl = titleUrl, id = id }));
            linkBuilder.MergeAttributes(new RouteValueDictionary(linkAttributes), true);
            var text = linkBuilder.ToString(TagRenderMode.StartTag);
            text += imgBuilder.ToString(TagRenderMode.SelfClosing);
            text += linkBuilder.ToString(TagRenderMode.EndTag);
            return MvcHtmlString.Create(text);
        }

        public static MvcHtmlString TipImageActionLink(this HtmlHelper html, string controllerName, string imageUrl, string altText, int id, string title, string titleUrl, object imgAttributes = null, object linkAttributes = null)
        {
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", imageUrl);
            imgBuilder.MergeAttribute("alt", altText);
            imgBuilder.MergeAttributes(new RouteValueDictionary(imgAttributes), true);
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext, html.RouteCollection);
            var linkBuilder = new TagBuilder("a");
            linkBuilder.MergeAttribute("href", urlHelper.Action("Details", controllerName, new { area = (string)null, tipTitleUrl = titleUrl, id = id }));
            linkBuilder.MergeAttributes(new RouteValueDictionary(linkAttributes), true);
            var text = linkBuilder.ToString(TagRenderMode.StartTag);
            text += imgBuilder.ToString(TagRenderMode.SelfClosing);
            text += linkBuilder.ToString(TagRenderMode.EndTag);
            return MvcHtmlString.Create(text);
        }

        public static MvcHtmlString RecipeImageActionLink(this HtmlHelper html, string controllerName, string imageUrl, string altText, int id, string title, string titleUrl, object imgAttributes = null, object linkAttributes = null)
        {
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", imageUrl);
            imgBuilder.MergeAttribute("alt", altText);
            imgBuilder.MergeAttributes(new RouteValueDictionary(imgAttributes), true);
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext, html.RouteCollection);
            var linkBuilder = new TagBuilder("a");
            linkBuilder.MergeAttribute("href", urlHelper.Action("Details", controllerName, new { area = (string)null, recipeTitleUrl = titleUrl, id = id }));
            linkBuilder.MergeAttributes(new RouteValueDictionary(linkAttributes), true);
            var text = linkBuilder.ToString(TagRenderMode.StartTag);
            text += imgBuilder.ToString(TagRenderMode.SelfClosing);
            text += linkBuilder.ToString(TagRenderMode.EndTag);
            return MvcHtmlString.Create(text);
        }
        
        public static MvcHtmlString QuestionLink(this HtmlHelper html, int id, string title, string titleUrl, object htmlAttributes = null)
        {
            return html.ActionLink(title, "Details", "Questions", new { area = (string)null, questionTitleUrl = titleUrl, id = id }, htmlAttributes);
        }

        public static MvcHtmlString UserLink(this HtmlHelper html, int id, string username, object htmlAttributes = null)
        {
            return html.ActionLink(username, "Details", "Account", new { area = (string)null, username = username.ToUrlFriendly(), id = id }, htmlAttributes);
        }

        public static MvcHtmlString TipLink(this HtmlHelper html, int id, string title, string titleUrl, object htmlAttributes = null)
        {
            return html.ActionLink(title, "Details", "Tips", new { area = (string)null, tipTitleUrl = titleUrl, id = id }, htmlAttributes);
        }

        public static MvcHtmlString TipCategoryLink(this HtmlHelper html, string title, int tipCategoryid, string name, string sort = null, object htmlAttributes = null)
        {
            return html.ActionLink(title, "TipCategoryIndex", "Tips", new { area = (string)null, tipCategoryNameUrl = name.ToUrlFriendly(), id = tipCategoryid, sort = sort }, htmlAttributes);
        }

        public static MvcHtmlString RecipeCategoryLink(this HtmlHelper html, string title, int categoryId, string name, string sort = null, object htmlAttributes = null)
        {
            return html.ActionLink(title, "RecipeCategoryIndex", "Recipes", new { area = (string)null, recipeCategoryNameUrl = name.ToUrlFriendly(), id = categoryId, sort = sort }, htmlAttributes);
        }

        public static MvcHtmlString RecipeSmallCategoryLink(this HtmlHelper html, string title, int categoryId, int smallCategoryId, string name, string sort = null, object htmlAttributes = null)
        {
            return html.ActionLink(title, "RecipeSmallCategoryIndex", "Recipes", new { area = (string)null, recipeCategoryNameUrl = name.ToUrlFriendly(), id = categoryId, smallId = smallCategoryId, sort = sort }, htmlAttributes);
        }

        public static MvcHtmlString RecipeLink(this HtmlHelper html, int id, string title, string titleUrl, object htmlAttributes = null)
        {
            return html.ActionLink(title, "Details", "Recipes", new { area = (string)null, recipeTitleUrl = titleUrl, id = id }, htmlAttributes);
        }
    }
}