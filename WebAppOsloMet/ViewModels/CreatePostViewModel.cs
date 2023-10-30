using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
    //  ViewModel which includes post for model, and a List of SelectListItems which is used for dropdownlist.
    public class CreatePostViewModel
    {
        public Post Post{ get; set; } = default!;
        public List<SelectListItem> SubForumSelectList { get; set; } = default!;
    }
}
