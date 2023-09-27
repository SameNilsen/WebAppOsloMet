using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
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
