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
using Humanizer;

namespace WebAppOsloMet.Controllers
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PostDbContext _postDbContext;

        //  Constructor. Initializes for example all repositories used.
        public PostController(ILogger<PostController> logger, IPostRepository postRepository, IUserRepository userRepository, UserManager<IdentityUser> userManager, PostDbContext postDbContext)
        {
            _logger = logger;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _postDbContext = postDbContext;
        }

        //  Action method used for getting all the posts in the database and return a view with this.
        public async Task<IActionResult> Posts()
        {
            var posts = await _postRepository.GetAll(); //  Uses repository to get posts.

            if (posts == null)   //  Error handling.
            {
                _logger.LogError("[PostController] List of posts not found while executing" +
                    "_postRepository.GetAll()");
                return NotFound("List of posts not found");
            }          

            ViewData["Votes"] = getVoteViewData(posts).Result;  //  Gets all votes from the logged in user and puts it in a ViewData object.

            var postListViewModel = new PostListViewModel(posts, "Table");  //  Create a ViewModel with the posts and a name.
            return View(postListViewModel);
        }

        //  Works much in the same way as Posts().
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

        //  Action method for fetching a given post from the database to be viewed in a detailed view.
        public async Task<IActionResult> DetailedPost(int id)
        {            
            var post = await _postRepository.GetItemById(id);  //  Gets post from database with method from repo.
            if (post == null)
            {
                _logger.LogError("[PostController] Post not found for the PostID {PostID:0000}", id);
                return NotFound("Post not found for this PostID");
            }
            var user = await _userRepository.GetItemById(post.UserId);  //  Gets the user who posted this post.
            if (user == null)
            {
                _logger.LogError("[PostController] User not found for this PostID {PostID:0000}", id);
                return NotFound("User not found for showing this PostID");
            }
            ViewData["Vote"] = GetOneVoteViewData(post).Result;  //  Gets the vote the logged in user voted.
           
            var postDetailViewModel = new PostDetailsViewModel(post, user);  //  Creates a ViewModel with the post and user as arguments.
            return View(postDetailViewModel);
        }

        //  Method for fetching the votes the logged in user voted on each post provided.
        public async Task<List<string>> getVoteViewData(IEnumerable<Post> posts)
        {
            var votes = new List<string>();  //  Prepares a list.

            //  <--- This block is for getting both the User user and IdentityUser user. We need the
            //       IdentityUser because we only want the votes given by the currently logged
            //       logged in user. 
            var identityUserId = _userManager.GetUserId(User);  //  Gets IdentityUser.
            var user = _userRepository.GetUserByIdentity(identityUserId).Result;  //  Uses the IdentityUser to get User user.
            if (user == null)  //  If a link between the IdentityUser and User has not been made yet a new User is created.
            {
                var newUser = new User
                {
                    Name = _userManager.GetUserName(User),
                    IdentityUserId = identityUserId
                };
                await _userRepository.Create(newUser);
            }
            user = _userRepository.GetUserByIdentity(identityUserId).Result;
            //  --->

            foreach (var post in posts)  // For loop to get the vote from each post.
            {
                if (post.UserVotes != null)  //  Check to see if there even are any votes on the post.
                {
                    //  If there is a vote from the logged in user on the post, add it to the list.
                    if (post.UserVotes.Exists(x => x.UserId == user.UserId && x.Post == post))
                    {
                        //  Warning: Well, we have already checked if the vote exists, and it does
                        //            so it will never be a null reference.
                        votes.Add(post.UserVotes.FirstOrDefault(x => x.UserId == user.UserId && x.Post == post).Vote);
                    }
                    //  If not then the user has not voted yet.
                    else
                    {
                        votes.Add("blank");
                    }
                }
                else { votes.Add("error"); }
            }
            
            return votes;
        }

        //  Much the same as getVoteViewData() above, but instead for only one post.
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
            
            return vote;
        }

        //  This Action method is used for getting the posts belonging to a subforum.
        public async Task<IActionResult> SubForumPosts(string CurrentViewName)
        {
            
            var subForum = CurrentViewName;
            
            //  The post repository has a method for getting all posts matching a query.
            var posts = _postRepository.GetBySubForum(subForum);
            if (posts == null)
            {
                _logger.LogError("[PostController] List of posts not found while executing" +
                    " _postRepository.GetBySubForum(subForum)");
                return NotFound("List of posts not found");
            }

            ViewData["Votes"] = getVoteViewData(posts).Result;  //  Getting the votes for each post by the logged in user.

            var subForums = new List<string>()  //  A list of the availible subforums.
            {
                "Gaming",
                "Sport",
                "School",
                "Nature",
                "Politics",
                "General"
            };
            //  A ViewModel is created with the posts, the subforum name and a list of SelectListItem
            //   to be used in a dropdownlist.
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
            ViewBag.RedirectForum = "Gaming";  //  Tror ikke blir brukt??

            return View(subForumPostListViewModel);
        }


        //  An action method for the page used when creating a new post.
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            
            var subForums = new List<string>()  //  A list of the availible subforums.
            {
                "Gaming",
                "Sport",
                "School",
                "Nature",
                "Politics",
                "General"
            };

            //  A ViewModel is created with a post to be used as model for Create form,
            //   and a list of SelectListItem to be used in a dropdownlist.
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

        //  The Post version of Create method, to be used when validating and saving post to database.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Post post)
        {
            try
            {
                //  <--- This block is for getting both the User user and IdentityUser user. We need the
                //       IdentityUser because then we can automatically assign the user as the 
                //        logged in user.
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
                    post.User = newUser;
                }
                else
                {
                    post.User = user;
                }
                //  --->

                var newPost = new Post  //  Creates a new post based on the form data.
                {
                    Title = post.Title,
                    Text = post.Text,
                    ImageUrl = post.ImageUrl,
                    PostDate = DateTime.Now.ToString("dd.MM.yyyy\nHH:mm:ss"),
                    UserId = post.UserId,
                    User = post.User,
                    SubForum = post.SubForum
                };

                
                ModelState.Remove("post.User");  //  This is set in this method, so we dont want to validate this part of the form data.
                if (ModelState.IsValid)  //  Checks if the post is created correctly.
                {
                    bool returnOk = await _postRepository.Create(newPost);
                    if (returnOk)
                    {
                        post.User.Credebility += 7;  //  When creating a post the user gets added score of 7 to their "Credebility".
                        await _userRepository.Update(post.User);
                        return RedirectToAction(nameof(Posts));
                    }
                }
                //  If creation fails, a log message is generated and the page redirects back.
                _logger.LogWarning("[PostController] Post creation failed {@post}", post);
                IActionResult view = Create();
                return view;
            }
            catch
            {
                return BadRequest("Could not create post... ");
            }
        }

        //  Action method for updating a post. This Get method returns the view for updating.
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Update(int id)
        {
            var post = await _postRepository.GetItemById(id);  //  Fetches the post.
            if (post == null)
            {
                _logger.LogError("[PostController] Post not found when updating the " +
                    "PostID {PostID:0000}", id);
                return BadRequest("Post not found for the PostID");
            }
            return View(post);
        }

        //  Post method for updating. The edited post comes as argument and its validity is checked.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update(Post post)
        {
            
            ModelState.Remove("User");  //  Again as in Create(), we dont want to check the user. This is already validated in creation, and is not able to be changed in update anyways.

            if (ModelState.IsValid)
            {
                bool returnOk = await _postRepository.Update(post);
                if (returnOk)
                    return RedirectToAction(nameof(DetailedPost), new { id = post.PostID});
            }
            _logger.LogWarning("[PostController] Post update failed {@post}", post);
            return View(post);
        }

        //  Delete method. 
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            //var item = await _itemDbContext.Items.FindAsync(id);
            var post = await _postRepository.GetItemById(id);  //  Gets the post to be deleted.
            if (post == null)
            {
                _logger.LogError("[PostController] Post not found for the PostID {PostID:0000}", id);
                return BadRequest("Post not found for the PostID");
            }
            return View(post);
        }

        //  The Post method where the post is deleted.
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
     
        //  A supporting method for getting the vote of a post.
        public async Task<Upvote> GetVote(Post post)
        {
            //  <--- This block is for getting both the User user and IdentityUser user. We need the
            //       IdentityUser because then we can automatically assign the user as the 
            //        logged in user.
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
            //  --->
            if (post.UserVotes != null)  //  Check to see if there are any votes on that post.
            {
                //  Finds and returns the vote.
                if (post.UserVotes.Exists(x => x.UserId == user.UserId && x.Post == post))
                {
                    return post.UserVotes.FirstOrDefault(x => x.UserId == user.UserId && x.Post == post);
                }
            }
            //  If the user has not voted on this post before, a new vote element is created.
            var newVote = new Upvote
            {
                UserId = user.UserId,
                User = user,
                PostID = post.PostID,
                Post = post
            };
            
            return newVote;
        }

        //  An action method called when a user upvotes a post.
        [Authorize]
        public async Task<IActionResult> UpVote(int id)  
        {
            
            var post = await _postRepository.GetItemById(id);  //  Finds the post in question.
           
            var vote = GetVote(post).Result;  //  Uses the GetVote() method to see and/or get the previous vote.

            //  If the user has upvoted the post before then nothing should happen. Else if
            //   the user has not voted or the previous vote was a downvote:
            if (vote.Vote == string.Empty || vote.Vote == "downvote") 
            {
                //  If the user has downvoted before and then upvotes, then the votecount
                //   should increment by two. If this is the first vote for the user on 
                //    this post the votecount should only be increased by one.
                if (vote.Vote == "downvote"){ post.UpvoteCount = post.UpvoteCount + 2; }
                else { post.UpvoteCount++; }

                await _postRepository.Update(post);  //  First update the database with the new votecount.
                
                vote.Vote = "upvote";  //  Set the new vote to upvote.
                _postDbContext.Upvotes.Update(vote);  // Then update the vote in the database.
                await _postDbContext.SaveChangesAsync();  //  Save it all.

                post.User.Credebility += 9;  //  When a post gets upvoted, the posts poster gets added "Credebility".
                await _userRepository.Update(post.User);
            }
            ViewBag.Vote = "Hei herfra upp";  //  Tror ikke er i bruk??            
            
            //  A bug when upvoting a post from the subforum page forces us to use this method for
            //   redirecting back to the page. Explanation in documentation.
            return Redirect(Request.Headers["Referer"].ToString());
        }

        //  An action method called when a user downvotes a post.
        [Authorize]
        public async Task<IActionResult> DownVote(int id)
        {
            
            var post = await _postRepository.GetItemById(id);  //  Finds the post in question.

            var vote = GetVote(post).Result;  //  Uses the GetVote() method to see and/or get the previous vote.

            //  If the user has downvoted the post before then nothing should happen. Else if
            //   the user has not voted or the previous vote was an upvote:
            if (vote.Vote == string.Empty || vote.Vote == "upvote")
            {
                //  If the user has upvoted before and then downvotes, then the votecount
                //   should decreased by two. If this is the first vote for the user on 
                //    this post the votecount should only be decreased by one.
                if (vote.Vote == "upvote") { post.UpvoteCount = post.UpvoteCount - 2; }
                else { post.UpvoteCount--; }

                await _postRepository.Update(post);  //  First update the database with the new votecount.
                
                vote.Vote = "downvote";  //  Set the new vote to downvote.
                _postDbContext.Upvotes.Update(vote);  // Then update the vote in the database.
                await _postDbContext.SaveChangesAsync();  //  Save it all.

                post.User.Credebility -= 4;  //  When a post gets downvoted, the posts poster loses "Credebility".
                await _userRepository.Update(post.User);
            }

            //  A bug when downvoting a post from the subforum page forces us to use this method for
            //   redirecting back to the page. Explanation in documentation.
            return Redirect(Request.Headers["Referer"].ToString());
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}