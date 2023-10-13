using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
    public class SubForumPostListViewModel
    {
        public IEnumerable<Post> Posts;
        public string? CurrentViewName;
        public List<SelectListItem> SubForumSelectList;

        public SubForumPostListViewModel(IEnumerable<Post> posts, string? currentViewName, List<SelectListItem> subForumSelectList)
        {
            Posts = posts;
            CurrentViewName = currentViewName;
            SubForumSelectList = subForumSelectList;
        }
    }
}
