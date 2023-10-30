using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
    //  ViewModel which includes a viewname and a list (IEnumerable) of posts to be used when displaying a list of posts.

    public class PostListViewModel
    {
        public IEnumerable<Post> Posts;
        public string? CurrentViewName;

        public PostListViewModel(IEnumerable<Post> posts, string? currentViewName)
        {
            Posts = posts;
            CurrentViewName = currentViewName;
        }
    }
}
