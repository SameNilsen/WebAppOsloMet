using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Diagnostics;
using WebAppOsloMet.ViewModels;
using WebAppOsloMet.DAL;
using WebAppOsloMet.Models;
using System.ComponentModel.DataAnnotations;

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
            if (posts == null)
            {
                _logger.LogError("[PostController] List of posts not found while executing" +
                    "_postRepository.GetAll()");
                return NotFound("List of posts not found");
            }

            //  <----- Everything in this block is only for colors on upvote button :(
            //            Maybe it can be moved to PostListModel or something idunno idc...
            //            NB: I moved it to a separate function instead.

            ViewData["Votes"] = getVoteViewData(posts).Result;

            //  ----->
            var postListViewModel = new PostListViewModel(posts, "Table");  //  Burde endres til PostListViewModel
            return View(postListViewModel);
        }

        public async Task<IActionResult> Card()
        {
            //var items = GetItems();    //  Gamle metode (uten db)
            var posts = await _postRepository.GetAll();
            if (posts == null)
            {
                _logger.LogError("[PostController] List of posts not found while executing" +
                    "_postRepository.GetAll()");
                return NotFound("List of posts not found");
            }
            
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
                _logger.LogError("[PostController] Post not found for the PostID {PostID:0000}", id);
                return NotFound("Post not found for this PostID");
            }
            var user = await _userRepository.GetItemById(post.UserId);
            if (user == null)
            {
                _logger.LogError("[PostController] User not found for this PostID {PostID:0000}", id);
                return NotFound("User not found for showing this PostID");
            }
            ViewData["Vote"] = GetOneVoteViewData(post).Result;

            //     CREATE VIEWMODEL TO INCLUDE NAME:
            var postDetailViewModel = new PostDetailsViewModel(post, user);
            return View(postDetailViewModel);
        }

        public async Task<List<string>> getVoteViewData(IEnumerable<Post> posts)
        {
            var votes = new List<string>();
            var identityUserId = _userManager.GetUserId(User);
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
            foreach (var post in posts)
            {
                if (post.UserVotes != null)
                {
                    if (post.UserVotes.Exists(x => x.UserId == user.UserId && x.Post == post))
                    {
                        votes.Add(post.UserVotes.FirstOrDefault(x => x.UserId == user.UserId && x.Post == post).Vote);
                    }
                    else
                    {
                        votes.Add("blank");
                    }
                }
                else { votes.Add("error"); }
            }
            Console.WriteLine("VOTES: " + votes.ToArray());
            votes.ForEach(Console.WriteLine);
            return votes;
        }

        public async Task<string> GetOneVoteViewData(Post post)
        {
            var vote = "blank";
            var identityUserId = _userManager.GetUserId(User);
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
            if (post.UserVotes != null)
            {
                if (post.UserVotes.Exists(x => x.UserId == user.UserId && x.Post == post))
                {
                    vote = post.UserVotes.FirstOrDefault(x => x.UserId == user.UserId && x.Post == post).Vote;
                }
                else
                {
                    vote = "blank";
                }
            }
            else { vote = "error"; }
            Console.WriteLine("VOTE: " + vote);
            //votes.ForEach(Console.WriteLine);
            return vote;
        }

        public async Task<IActionResult> SubForumPosts(string CurrentViewName)
        {
            Console.WriteLine(":::"+CurrentViewName);
            var subForum = CurrentViewName;
            Console.WriteLine("----:"+ CurrentViewName + ":" + subForum);

            var posts = _postRepository.GetBySubForum(subForum);
            if (posts == null)
            {
                _logger.LogError("[PostController] List of posts not found while executing" +
                    " _postRepository.GetBySubForum(subForum)");
                return NotFound("List of posts not found");
            }

            ViewData["Votes"] = getVoteViewData(posts).Result;
            var subForums = new List<string>()  //  Could(should) be stored in database instead iguess.
            {
                "Gaming",
                "Sport",
                "School",
                "Nature",
                "Politics",
                "General"
            };
            var subForumPostListViewModel = new SubForumPostListViewModel
            (
                posts,
                subForum,
                subForums.Select(forum => new SelectListItem
                {
                    Value = forum.ToString(),
                    Text = forum.ToString().ToUpper()
                }).ToList()
            );
            ViewBag.RedirectForum = "Gaming";

            return View(subForumPostListViewModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            //Console.WriteLine(_userManager.GetUserName(User));
            //  Create SubForumSelectList
            var subForums = new List<string>()  //  Could(should) be stored in database instead iguess.
            {
                "Gaming",
                "Sport",
                "School",
                "Nature",
                "Politics",
                "General"
            };
            var createPostViewModel = new CreatePostViewModel
            {
                Post = new Post(),
                SubForumSelectList = subForums.Select(forum => new SelectListItem
                {
                    Value = forum.ToString(),
                    Text = forum.ToString().ToUpper()
                }).ToList(),
            };
            return View(createPostViewModel);
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
                    User = post.User,
                    SubForum = post.SubForum
                };

                //var newModel = new ValidationContext(newPost);
                ModelState.Remove("post.User");
                if (ModelState.IsValid)
                {
                    bool returnOk = await _postRepository.Create(post);
                    if (returnOk)
                    {
                        user.Credebility += 7;
                        await _userRepository.Update(user);
                        return RedirectToAction(nameof(Posts));
                    }
                }
                _logger.LogWarning("[PostController] Post creation failed {@post}", post);
                IActionResult view = Create();
                return view;
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
            var post = await _postRepository.GetItemById(id);
            if (post == null)
            {
                _logger.LogError("[PostController] Post not found when updating the " +
                    "PostID {PostID:0000}", id);
                return BadRequest("Post not found for the PostID");
            }
            return View(post);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(Post post)
        {
            Console.WriteLine(post.PostID + "-----------");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                bool returnOk = await _postRepository.Update(post);
                if (returnOk)
                    return RedirectToAction(nameof(DetailedPost), new { id = post.PostID});
            }
            _logger.LogWarning("[PostController] Post update failed {@post}", post);
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
                _logger.LogError("[PostController] Post not found for the PostID {PostID:0000}", id);
                return BadRequest("Post not found for the PostID");
            }
            return View(post);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool returnOk = await _postRepository.Delete(id);
            if (!returnOk)
            {
                _logger.LogError("[PostController] Post deletion failed for the PostId {PostID:0000}", id);
                return BadRequest("Post deletion failed");
            }
            return RedirectToAction(nameof(Posts));
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
                post.User.Credebility += 9;
                await _userRepository.Update(post.User);
            }
            ViewBag.Vote = "Hei herfra upp";

            //  EN TEST FOR Å FIKSE REDIRECT NÅR MAN UP/DOWNVOTER FRA SUBFORUM
            //String[] spearator = { "/" };
            //var referer = Request.Headers["Referer"].ToString().Split(spearator, StringSplitOptions.RemoveEmptyEntries);
            //var redirect = referer.GetValue(referer.Length - 1);
            //return RedirectToAction(redirect.ToString(), "post", new { CurrentViewName = post.SubForum.ToString() });
            
            return Redirect(Request.Headers["Referer"].ToString());
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
                post.User.Credebility -= 4;
                await _userRepository.Update(post.User);
            }
            //String[] spearator = { "/" };
            //var referer = Request.Headers["Referer"].ToString().Split(spearator, StringSplitOptions.RemoveEmptyEntries);
            //var redirect = referer.GetValue(referer.Length - 1);
            //return RedirectToAction(redirect.ToString(), "post", new { CurrentViewName = post.SubForum.ToString() });
            
            return Redirect(Request.Headers["Referer"].ToString());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}