using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using WebAppOsloMet.DAL;
using WebAppOsloMet.Models;
using WebAppOsloMet.ViewModels;

namespace WebAppOsloMet.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;

        public PostController(ILogger<PostController> logger, IPostRepository postRepository, IUserRepository userRepository)
        {
            _logger = logger;
            _postRepository = postRepository;
            _userRepository = userRepository;
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

        public async Task<IActionResult> DetailedPost(int id)
        {
            //List<Item> items = _itemDbContext.Items.ToList();
            //var item = await _itemDbContext.Items.FirstOrDefaultAsync(i => i.ItemID == id);
            var post = await _postRepository.GetItemById(id);
            if (post == null)
            {
                return NotFound();
            }
            var user = await _userRepository.GetItemById(post.UserId);
            //     CREATE VIEWMODEL TO INCLUDE NAME:
            var postDetailViewModel = new PostDetailsViewModel(post, user);
            return View(postDetailViewModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Post post)
        {
            try
            {
                var user = _userRepository.GetItemById(post.UserId).Result;
                if (user == null)
                {
                    return BadRequest("User not found!");
                }
                post.User = user;
                var newPost = new Post
                {
                    Title = post.Title,
                    Text = post.Text,
                    ImageUrl = post.ImageUrl,
                    PostDate = DateTime.Today.ToString(),
                    UserId = post.UserId,
                    User = user
                };            
                await _postRepository.Create(post);
                return RedirectToAction(nameof(Posts));
            }
            catch
            {
                return BadRequest("Could not create post... ");
            }
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}