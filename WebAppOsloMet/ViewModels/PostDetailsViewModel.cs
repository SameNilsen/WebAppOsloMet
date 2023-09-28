using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
    public class PostDetailsViewModel
    {
        public Post Post;
        public User User;

        public PostDetailsViewModel(Post post, User user)
        {
            Post = post;
            User = user;
        }
    }
}
