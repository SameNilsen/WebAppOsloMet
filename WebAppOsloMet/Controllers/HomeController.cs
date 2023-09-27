using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppOsloMet.DAL;
using WebAppOsloMet.Models;
using WebAppOsloMet.ViewModels;

namespace WebAppOsloMet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IItemRepository _itemRepository;

        public HomeController(ILogger<HomeController> logger, IItemRepository itemRepository)
        {
            _logger = logger;
            _itemRepository = itemRepository;
        }

        public async Task<IActionResult> Posts()
        {
            //var items = GetItems();    //  Gamle metode (uten db)
            //List<Item> items = await _itemDbContext.Items.ToListAsync();  //  Uten repo pattern
            var items = await _itemRepository.GetAll();
            var itemListViewModel = new ItemListViewModel(items, "Table");
            return View(itemListViewModel);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}