using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
    //  ViewModel which includes post and user for model to be used when displaying a post in a detailed view.

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
