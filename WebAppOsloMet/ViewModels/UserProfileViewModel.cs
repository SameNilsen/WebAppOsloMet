using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
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
