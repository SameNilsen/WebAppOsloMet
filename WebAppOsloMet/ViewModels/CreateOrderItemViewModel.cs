using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppOsloMet.Models;

namespace WebAppOsloMet.ViewModels
{
    public class CreateOrderItemViewModel
    {
        public OrderItem OrderItem { get; set; } = default!;
        public List<SelectListItem> ItemSelectList { get; set; } = default!;
        public List<SelectListItem> OrderSelectList { get; set; } = default!;
    }
}
