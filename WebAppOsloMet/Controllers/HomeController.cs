using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            var itemListViewModel = new ItemListViewModel(items, "Table");  //  Burde endres til PostListViewModel
            return View(itemListViewModel);
        }

        public async Task<IActionResult> Grid()
        {
            //var items = GetItems();    //  Gamle metode (uten db)
            var items = await _itemRepository.GetAll();
            var itemListViewModel = new ItemListViewModel(items, "Grid");
            return View(itemListViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            //List<Item> items = _itemDbContext.Items.ToList();
            //var item = await _itemDbContext.Items.FirstOrDefaultAsync(i => i.ItemID == id);
            var post = await _itemRepository.GetItemById(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                //_itemDbContext.Items.Add(item);
                //await _itemDbContext.SaveChangesAsync();
                await _itemRepository.Create(post);
                return RedirectToAction(nameof(Table));
            }
            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var item = await _itemDbContext.Items.FindAsync(id);
            var post = await _itemRepository.GetItemById(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Item post)
        {
            if ((ModelState.IsValid))
            {
                //_itemDbContext.Items.Update(item);
                //await _itemDbContext.SaveChangesAsync();
                await _itemRepository.Update(post);
                return RedirectToAction(nameof(Table));
            }
            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //var item = await _itemDbContext.Items.FindAsync(id);
            var post = await _itemRepository.GetItemById(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var item = await _itemDbContext.Items.FindAsync(id);
            //if (item == null)
            //{
            //    return NotFound();
            //}
            //_itemDbContext.Items.Remove(item);
            //await _itemDbContext.SaveChangesAsync();
            await _itemRepository.Delete(id);
            return RedirectToAction(nameof(Table));
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