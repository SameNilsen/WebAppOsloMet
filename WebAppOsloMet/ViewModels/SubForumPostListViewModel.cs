using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
    //  ViewModel which includes a viewname and a list (IEnumerable) of posts to be used when displaying a list of posts, along with a
    //   list of SelectListItems for selection of subforum in a dropdownlist.

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
