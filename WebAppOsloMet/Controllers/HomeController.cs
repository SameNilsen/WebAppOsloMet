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
        private readonly IPostRepository _postRepository;

        public HomeController(ILogger<HomeController> logger, IPostRepository postRepository)
        {
            _logger = logger;
            _postRepository = postRepository;
        }

        public async Task<IActionResult> Posts()
        {
            //var items = GetItems();    //  Gamle metode (uten db)
            //List<Item> items = await _itemDbContext.Items.ToListAsync();  //  Uten repo pattern
            var posts = await _postRepository.GetAll();
            var postListViewModel = new PostListViewModel(posts, "Table");  //  Burde endres til PostListViewModel
            return View(postListViewModel);
        }

        public async Task<IActionResult> Grid()
        {
            //var items = GetItems();    //  Gamle metode (uten db)
            var posts = await _postRepository.GetAll();
            var postListViewModel = new PostListViewModel(posts, "Grid");
            return View(postListViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            //List<Item> items = _itemDbContext.Items.ToList();
            //var item = await _itemDbContext.Items.FirstOrDefaultAsync(i => i.ItemID == id);
            var post = await _postRepository.GetItemById(id);
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
                await _postRepository.Create(post);
                return RedirectToAction(nameof(Table));
            }
            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var item = await _itemDbContext.Items.FindAsync(id);
            var post = await _postRepository.GetItemById(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Post post)
        {
            if ((ModelState.IsValid))
            {
                //_itemDbContext.Items.Update(item);
                //await _itemDbContext.SaveChangesAsync();
                await _postRepository.Update(post);
                return RedirectToAction(nameof(Table));
            }
            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //var item = await _itemDbContext.Items.FindAsync(id);
            var post = await _postRepository.GetItemById(id);
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
            await _postRepository.Delete(id);
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