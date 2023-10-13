using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
    public class CreatePostViewModel
    {
        public Post Post{ get; set; } = default!;
        public List<SelectListItem> SubForumSelectList { get; set; } = default!;
    }
}
