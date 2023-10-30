using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
    //  ViewModel which includes a list (IEnumerable) of posts, comments and votes to be used when displaying a list of posts along with a User.

    public class UserProfileViewModel
    {
        public IEnumerable<Post>? Posts;
        public IEnumerable<Comment>? Comments;
        public IEnumerable<Upvote>? Votes;
        public User User;

        public UserProfileViewModel(IEnumerable<Post>? posts, IEnumerable<Comment>? comments, IEnumerable<Upvote>? votes, User user)
        {
            Posts = posts;
            Comments = comments;
            Votes = votes;
            User = user;
        }
    }
}
