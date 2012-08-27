using Microsoft.Practices.Unity;
using CRS.Business.Interfaces;
using CRS.Business.Repositories;

namespace CRS.Business.Configuration
{
    /// <summary>
    /// Includes configuration for CRS.Business assembly to be used with Unity Container
    /// </summary>
    public class BusinessUnityExtension : UnityContainerExtension
    {
        private void RegisterRepositories()
        {
            // All repositories should be singleton
            Container
                .RegisterType<INewsRepository, NewsRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<ISecurityRepository, SecurityRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IUserRepository, UserRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IAnswerRepository, AnswerRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IQuestionRepository, QuestionRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<INewsCommentRepository, NewsCommentRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<ITipCommentRepository, TipCommentRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<ITipRepository, TipRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<ITipCategoryRepository, TipCategoryRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IApplicationRepository, ApplicationRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IReferenceDataRepository, ReferenceDataRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IAllContentRepository, AllContentRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IRecipeRepository, RecipeRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IRecipeCategoryRepository, RecipeCategoryRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IRecipeSmallCategoryRepository, RecipeSmallCategoryRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IRecipeAllCategoryRepository, RecipeAllCategoryRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IRecipeCommentRepository, RecipeCommentRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IWordRepository, WordRepository>(new ContainerControlledLifetimeManager())
                .RegisterType<IUpdatedWordRepository, UpdatedWordRepository>(new ContainerControlledLifetimeManager());
        }

        #region Overrides of UnityContainerExtension

        protected override void Initialize()
        {
            RegisterRepositories();
        }

        #endregion
    }

}