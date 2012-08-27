using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    /// <summary>
    /// Performs operations on users
    /// </summary>
    public interface IUserRepository
    {
        //Manage Users
        Feedback<IList<UserState>> GetAllUserStates();
        Feedback<IList<User>> GetAllUsers();
        Feedback<IList<User>> GetSimilarUsers(string username);
        Feedback<User> DeleteUser(int id);
        Feedback<User> GetUserDetails(int id);

        //User Profile
        Feedback<User> GetPersonalInfo(int id);
        Feedback<User> UpdateUser(User c, IList<int> roleIds);
        Feedback<string> UpdateUserProfile(User currentUser);
        Feedback<string> RemoveAvatar(int id);
        Feedback<User> ViewOtherUserInformaton(int id);
        Feedback EmailExists(string email);
        ListQuestionFeedback GetUserQuestion(int userid);
        ListTipFeedback GetUserTip(int userid);
        ListNewsFeedback GetUserNews(int userid);
        ListAnswerFeedback GetUserAnswer(int userid);
        Feedback<IList<User>> GetTopContributors(int take);
        ListRecipeFeedback GetUserRecipe(int userid);
        ListRecipeFeedback GetUserFavoriteRecipes(int userid);
        Feedback DeleteUserFavoriteRecipes(List<int> idList, int userId);
    }
}