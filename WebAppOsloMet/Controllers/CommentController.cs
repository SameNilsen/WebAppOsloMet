using Microsoft.AspNetCore.Mvc;
using WebAppOsloMet.Models;
using Microsoft.EntityFrameworkCore;
using WebAppOsloMet.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebAppOsloMet.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace WebAppOsloMet.Controllers
{
    public class CommentController : Controller
    {
        private readonly PostDbContext _postDbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentController(PostDbContext postDbContext, UserManager<IdentityUser> userManager)
        {
            _postDbContext = postDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Table(int id)
        {
            var user = await _postDbContext.Users.FindAsync(id);
            if (user == null)
            {
                return BadRequest("Did not find user");
            }
            List<Post> postss = user.Posts;
            List<Post> posts = await _postDbContext.Posts.ToListAsync();
            return View(postss);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Comment comment)
        {
            Console.WriteLine(comment.CommentText);
            Console.WriteLine(comment.PostID);

            try
            {
                //var identityUserId = _userManager.GetUserId(User);
                //var user = _userRepository.GetUserByIdentity(identityUserId).Result;
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

                var newComment = new Comment
                {
                    CommentText = comment.CommentText,
                    PostDate = DateTime.Today.ToString(),
                    UserId = comment.User.UserId,
                    User = comment.User,
                    PostID = comment.PostID,
                    Post = comment.Post
                };
                _postDbContext.Comments.Add(newComment);
                await _postDbContext.SaveChangesAsync();
                return RedirectToAction("DetailedPost", "Post", new { id = comment.PostID});
            }
            catch
            {
                return BadRequest("Could not create post... ");
            }
        }
    }
}
