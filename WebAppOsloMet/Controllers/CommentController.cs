using Microsoft.AspNetCore.Mvc;
using WebAppOsloMet.Models;
using Microsoft.EntityFrameworkCore;
using WebAppOsloMet.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebAppOsloMet.Controllers
{
    public class CommentController : Controller
    {
        private readonly PostDbContext _postDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CommentController> _logger;
        private readonly IUserRepository _userRepository;

        public CommentController(ILogger<CommentController> logger, PostDbContext postDbContext, UserManager<IdentityUser> userManager, IUserRepository userRepository)
        {
            _postDbContext = postDbContext;
            _userManager = userManager;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Table(int id)
        {
            var user = await _postDbContext.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogError("[CommentController] Did not find user {UserID:0000}", id);
                return NotFound("Did not find user");
            }
            List<Post>? postss = user.Posts;
            List<Post> posts = await _postDbContext.Posts.ToListAsync();
            return View(postss);
        }

        //  Action method for creating a new comment.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Comment comment)
        {            

            try
            {
                //  <--- This block is for getting both the User user and IdentityUser user. We need the
                //       IdentityUser because then we can automatically assign the user as the 
                //        logged in user.
                var identityUserId = _userManager.GetUserId(User);
                var user = _postDbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId).Result;

                if (user == null)
                {
                    var newUser = new User
                    {
                        Name = _userManager.GetUserName(User),
                        IdentityUserId = identityUserId
                    };
                    _postDbContext.Users.Add(newUser);
                    await _postDbContext.SaveChangesAsync();
                    comment.User = newUser;
                }
                else
                {
                    comment.User = user;
                }
                //  --->

                //  Creates the new comment based on the form data.
                var newComment = new Comment
                {
                    CommentText = comment.CommentText,
                    PostDate = DateTime.Now.ToString("dd.MM.yyyy HH:mm"),
                    UserId = comment.User.UserId,
                    User = comment.User,
                    PostID = comment.PostID,
                    Post = comment.Post
                };
                _postDbContext.Comments.Add(newComment);
                await _postDbContext.SaveChangesAsync();

                //  Set credibility for the commenter:
                newComment.User.Credebility += 3;
                await _userRepository.Update(newComment.User);

                //  Set credibility for the posts poster:
                var post = await _postDbContext.Posts.FindAsync(comment.PostID);
                if (post != null)
                {
                    //  If it cannot find the post, the poster will not get creds, which is not
                    //   detrimental.
                    post.User.Credebility += 5;
                    await _userRepository.Update(post.User);
                }
                //  Redirects back to the same page, that is the DetailedPost page, using the 
                //   action method DetailedPost() in the PostController.
                return RedirectToAction("DetailedPost", "Post", new { id = comment.PostID});
            }
            catch
            {
                _logger.LogError("[CommentController] Could not create comment for PostID {PostID:0000}", comment);
                return BadRequest("Could not create comment... ");
            }
        }

        public async Task<IActionResult> Update(Comment comment)
        {
            ModelState.Remove("User");
            ModelState.Remove("Post");

            if (ModelState.IsValid)
            {
                _postDbContext.Comments.Update(comment);
                await _postDbContext.SaveChangesAsync();
                
                return RedirectToAction("DetailedPost", "Post", new { id = comment.PostID });
            }
            _logger.LogWarning("[CommentController] Comment update failed {@comment}", comment);
            return RedirectToAction("DetailedPost", "Post", new { id = comment.PostID });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _postDbContext.Comments.FindAsync(id);
            if (comment == null)
            {
                _logger.LogError("[CommentController] comment not found for the CommentID {CommentID:0000}", id);
                return RedirectToAction("Posts", "Post");
            }
            _postDbContext.Comments.Remove(comment);
            await _postDbContext.SaveChangesAsync();
            return RedirectToAction("DetailedPost", "Post", new { id = comment.PostID });
        }
    }
}
