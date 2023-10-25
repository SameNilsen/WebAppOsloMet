using Microsoft.AspNetCore.Mvc;
using WebAppOsloMet.Models;
using Microsoft.EntityFrameworkCore;
using WebAppOsloMet.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppOsloMet.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebAppOsloMet.Controllers
{
    public class UserController : Controller
    {
        private readonly PostDbContext _postDbContext;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;

        public UserController(ILogger<UserController> logger, PostDbContext postDbContext, UserManager<IdentityUser> userManager, IUserRepository userRepository)
        {
            _postDbContext = postDbContext;
            _logger = logger;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        //  When a user clicks the posts button in the navbar, this method is used.
        public async Task<IActionResult> MyPosts(string id)
        {
            // GET USER FROM IDENTITYUSER
            var user = _postDbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == id).Result;
            if (user == null)
            {
                var newUser = new User
                {
                    Name = _userManager.GetUserName(User),
                    IdentityUserId = id
                };
                await _userRepository.Create(newUser);
            }
            user = _userRepository.GetUserByIdentity(id).Result;

            if (user.Posts.Count() == 0)  //  If the user has no posts to show, a dummy post is created.
            {                
                return View("Table", new List<Post>() { new Post { User = user, UserId = -1} });                
            }

            //  Else the viewpage of Table is returned along with the posts.
            List<Post> posts = user.Posts;
            _logger.LogWarning("This is a warning message!");
            return View("Table", posts);
        }

        //  When accessing all posts from a user:
        public async Task<IActionResult> Table(int id)
        {
            //  Finds the user:
            var user = await _postDbContext.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogError("[UserController] User not found while executing" +
                    "_postDbContext.Users.FindAsync(id)", id);
                return NotFound("Did not find user");
            }
            //  Returns the posts:
            List<Post> postss = user.Posts;
            List<Post> posts = await _postDbContext.Posts.ToListAsync();
            return View(postss);
        }

        //  This Action method is for when the user clicks into the userprofile page of a user.
        public async Task<IActionResult> UserProfile(int id)
        {
            User user;
            if (id == -1)
            {
                //  The user is the logged in user!!
                var identityUserId = _userManager.GetUserId(User);
                user = _userRepository.GetUserByIdentity(identityUserId).Result;
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
            }
            else
            {
                user = await _postDbContext.Users.FindAsync(id);
            }
            if (user == null)
            {
                _logger.LogError("[UserController] User not found while executing" +
                    "_postDbContext.Users.FindAsync(id)", id);
                return NotFound("Did not find user");
            }

            //  This method returns a ViewModel with all the users posts, comments and votes.
            List<Post> posts = user.Posts;
            List<Comment> comments = user.Comments;
            List<Upvote> votes = user.UserVotes;
            var userProfileViewModel = new UserProfileViewModel(posts, comments, votes, user);
            return View(userProfileViewModel);
        }

        //  When accessing the page for displaying all comments:
        public async Task<IActionResult> Comments(int userID)
        {            
            var user = await _userRepository.GetItemById(userID);
           
            if (user == null)
            {
                _logger.LogError("[UserController] User not found while executing" +
                    "_postDbContext.Users.FindAsync(id)", userID);
                return NotFound("Did not find user");
            }
            ViewData["User"] = user;  //  Add the user as ViewData for displaying name on the page.
            var comments = user.Comments;
            return View(comments);
        }

        //  When accessing the page for displaying all Votes:
        public async Task<IActionResult> Votes(int userID)
        {
            var user = await _userRepository.GetItemById(userID);

            if (user == null)
            {
                _logger.LogError("[UserController] User not found while executing" +
                    "_postDbContext.Users.FindAsync(id)", userID);
                return NotFound("Did not find user");
            }
            ViewData["User"] = user;
            var votes = user.UserVotes;
            return View(votes);
        }

        //  Action method for displaying page of Credibility system.
        public IActionResult CredsInfo()
        {           
            return View();
        }        
    }
}
