using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PostDbContext _postDbContext;

        public PostController(ILogger<PostController> logger, IPostRepository postRepository, IUserRepository userRepository, UserManager<IdentityUser> userManager, PostDbContext postDbContext)
        {
            _logger = logger;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _postDbContext = postDbContext;
        }

        public async Task<IActionResult> Posts()
        {
            //var items = GetItems();    //  Gamle metode (uten db)
            //List<Item> items = await _itemDbContext.Items.ToListAsync();  //  Uten repo pattern
            var posts = await _postRepository.GetAll();
            var postListViewModel = new PostListViewModel(posts, "Table");  //  Burde endres til PostListViewModel
            return View(postListViewModel);
        }

        public async Task<IActionResult> Card()
        {
            //var items = GetItems();    //  Gamle metode (uten db)
            var posts = await _postRepository.GetAll();
            var postListViewModel = new PostListViewModel(posts, "Card");
            return View(postListViewModel);
        }

        public async Task<IActionResult> DetailedPost(int id)
        {
            Console.WriteLine(id);
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
            //Console.WriteLine(_userManager.GetUserName(User));
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Post post)
        {
            try
            {
                var identityUserId = _userManager.GetUserId(User);
                var user = _userRepository.GetUserByIdentity(identityUserId).Result;
                //var user = _userRepository.GetItemById(post.UserId).Result;

                if (user == null)
                {
                    var newUser = new User
                    {
                        Name = _userManager.GetUserName(User),
                        IdentityUserId = identityUserId
                    };
                    await _userRepository.Create(newUser);
                    post.User = newUser;
                }
                else
                {
                    post.User = user;
                }

                var newPost = new Post
                {
                    Title = post.Title,
                    Text = post.Text,
                    ImageUrl = post.ImageUrl,
                    PostDate = DateTime.Today.ToString(),
                    UserId = post.UserId,
                    User = post.User
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

        //public async Task<string> CheckIfVoted(Post post)
        //{
        //    var identityUserId = _userManager.GetUserId(User);
        //    Console.WriteLine("usident" +  identityUserId);
        //    var user = _userRepository.GetUserByIdentity(identityUserId).Result;
        //    Console.WriteLine("USER: " + user);
        //    Console.WriteLine(" -- " + user.UserId);
        //    Console.WriteLine("---" + post.PostID);
        //    if (post.UserVotes != null)
        //    {
        //        if (post.UserVotes.Exists(x => x.UserId == user.UserId && x.Post == post && x.Vote == vote))
        //        {                    
        //            return true;
        //        }
        //    }            
        //    return false;
        //}

        public async Task<Upvote> GetVote(Post post)
        {
            var identityUserId = _userManager.GetUserId(User);
            Console.WriteLine("usident" + identityUserId);
            var user = _userRepository.GetUserByIdentity(identityUserId).Result;
            if (user == null)
            {
                var newUser = new User
                {
                    Name = _userManager.GetUserName(User),
                    IdentityUserId = identityUserId
                };
                await _userRepository.Create(newUser);             
            }
            user = _userRepository.GetUserByIdentity(identityUserId).Result;
            Console.WriteLine("USER: " + user);
            Console.WriteLine(" -- " + user.UserId);
            Console.WriteLine("---" + post.PostID);
            if (post.UserVotes != null)
            {
                if (post.UserVotes.Exists(x => x.UserId == user.UserId && x.Post == post))
                {
                    return post.UserVotes.FirstOrDefault(x => x.UserId == user.UserId && x.Post == post);
                }
            }

            var newVote = new Upvote
            {
                UserId = user.UserId,
                User = user,
                PostID = post.PostID,
                Post = post
            };
            
            return newVote;
        }

        [Authorize]
        public async Task<IActionResult> UpVote(int id)  //  eventuelt postID
        {
            Console.WriteLine("UPP");
            var post = await _postRepository.GetItemById(id);

            //if (CheckIfVoted(post).Result == "downvote")
            //{
            //    Console.WriteLine("UPVOTE");
            //    post.UpvoteCount++;
            //    await _postRepository.Update(post);
            //    //post.UserVotes.FirstOrDefault(x => x.UserId == user.UserId && x.Post == post

            //}
            var vote = GetVote(post).Result;
            Console.WriteLine("CURRENT VOTE: " + vote.Vote);
            if (vote.Vote == string.Empty || vote.Vote == "downvote")
            {
                Console.WriteLine("UPVOTE");
                if (vote.Vote == "downvote"){ post.UpvoteCount = post.UpvoteCount + 2; }
                else { post.UpvoteCount++; }
                await _postRepository.Update(post);
                vote.Vote = "upvote";
                _postDbContext.Upvotes.Update(vote);
                await _postDbContext.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Posts));
        }

        [Authorize]
        public async Task<IActionResult> DownVote(int id)  //  eventuelt postID
        {
            Console.WriteLine(id);
            Console.WriteLine("DOWNN" + id);
            var post = await _postRepository.GetItemById(id);

            var vote = GetVote(post).Result;
            if (vote.Vote == string.Empty || vote.Vote == "upvote")
            {
                Console.WriteLine("DOWNVOTE");
                if (vote.Vote == "upvote") { post.UpvoteCount = post.UpvoteCount - 2; }
                else { post.UpvoteCount--; }
                await _postRepository.Update(post);
                vote.Vote = "downvote";
                _postDbContext.Upvotes.Update(vote);
                await _postDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Posts));
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}