using System.Collections.Generic;
using CRS.Business.Models.Entities;

namespace CRS.Web.ViewModels.RecipeComments
{
    public class ListRecipeCommentViewModel
    {
        public int RecipeId { get; set; }
        public IList<RecipeComment> Comments { get; set; }
        public bool HasMore { get; set; }
        public int Total { get; set; }
        public int CurrentUserId { get; set; }
        public IList<RecipeComment> HighlightComments { get; set; }
    }
}