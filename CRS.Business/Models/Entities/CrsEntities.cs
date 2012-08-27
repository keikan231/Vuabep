using System.Data.Entity;

namespace CRS.Business.Models.Entities
{
    public class CrsEntities: DbContext
    {
        public DbSet<News> News { get; set; }
        public DbSet<Tip> Tips { get; set; }
        public DbSet<TipCategory> TipCategories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserState> UserStates { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<ReportedAnswer> ReportedAnswers { get; set; }
        public DbSet<ReportedQuestion> ReportedQuestions { get; set; }
        public DbSet<ReportedNews> ReportedNews { get; set; }
        public DbSet<ReportedTip> ReportedTips { get; set; }
        public DbSet<VotedAnswer> VotedAnswers { get; set; }
        public DbSet<NewsComment> NewsComments { get; set; }
        public DbSet<ReportedNewsComment> ReportedNewsComments { get; set; }
        public DbSet<VotedNewsComment> VotedNewsComments { get; set; }
        public DbSet<TagCategory> TagCategories { get; set; }
        public DbSet<TipComment> TipComments { get; set; }
        public DbSet<ReportedTipComment> ReportedTipComments { get; set; }
        public DbSet<VotedTipComment> VotedTipComments { get; set; }
        public DbSet<VisitorNumber> VisitorNumbers { get; set; }
        public DbSet<VotedTip> VotedTips { get; set; }
        public DbSet<PointConfig> PointConfigs { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<UpdatedWord> UpdatedWords { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeCategory> RecipeCategories { get; set; }
        public DbSet<RecipeSmallCategory> RecipeSmallCategories { get; set; }
        public DbSet<RecipeCategoryMapping> RecipeCategoryMappings { get; set; }
        public DbSet<ReportedRecipe> ReportedRecipes { get; set; }
        public DbSet<RecipeComment> RecipeComments { get; set; }
        public DbSet<ReportedRecipeComment> ReportedRecipeComments { get; set; }
        public DbSet<VotedRecipeComment> VotedRecipeComments { get; set; }
        public DbSet<RatedRecipe> RatedRecipes { get; set; }
        public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }
    }
}