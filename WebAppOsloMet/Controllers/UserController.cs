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
            if (user.Posts.Count() == 0)
            {
                Console.WriteLine("Yohoo");
                return View("Table", new List<Post>() { new Post { User = user, UserId = -1} });
                //return BadRequest("No posts...");
            }

            List<Post> posts = user.Posts;
            _logger.LogWarning("This is a warning message!");
            return View("Table", posts);
        }
        public async Task<IActionResult> Table(int id)
        {
            var user = await _postDbContext.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogError("[UserController] User not found while executing" +
                    "_postDbContext.Users.FindAsync(id)", id);
                return NotFound("Did not find user");
            }
            List<Post> postss = user.Posts;
            List<Post> posts = await _postDbContext.Posts.ToListAsync();
            return View(postss);
        }

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
            List<Post> posts = user.Posts;
            List<Comment> comments = user.Comments;
            List<Upvote> votes = user.UserVotes;
            var userProfileViewModel = new UserProfileViewModel(posts, comments, votes, user);
            return View(userProfileViewModel);
        }

        public async Task<IActionResult> Comments(int userID)
        {            
            var user = await _userRepository.GetItemById(userID);
           
            if (user == null)
            {
                _logger.LogError("[UserController] User not found while executing" +
                    "_postDbContext.Users.FindAsync(id)", userID);
                return NotFound("Did not find user");
            }
            ViewData["User"] = user;
            var comments = user.Comments;
            return View(comments);
        }

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

        public IActionResult CredsInfo()
        {           
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> CreateOrderItem()
        //{
        //    var items = await _itemDbContext.Items.ToListAsync();
        //    var orders = await _itemDbContext.Orders.ToListAsync();
        //    var createOrderItemViewModel = new CreateOrderItemViewModel
        //    {
        //        OrderItem = new OrderItem(),

        //        ItemSelectList = items.Select(item => new SelectListItem
        //        {
        //            Value = item.ItemID.ToString(),
        //            Text = item.ItemID.ToString() + ": " + item.Name
        //        }).ToList(),

        //        OrderSelectList = orders.Select(order => new SelectListItem
        //        {
        //            Value = order.OrderId.ToString(),
        //            Text = "Order" + order.OrderId.ToString() + ", Date: " + order.OrderDate +
        //            ", Customer: " + order.Customer.Name
        //        }).ToList()
        //    };
        //    return View(createOrderItemViewModel);
        //}

        //[HttpPost]
        //public async Task<IActionResult> CreateOrderItem(OrderItem orderItem)
        //{
        //    try
        //    {
        //        var newItem = _itemDbContext.Items.Find(orderItem.ItemId);
        //        var newOrder = _itemDbContext.Orders.Find(orderItem.OrderId);

        //        if (newItem == null || newOrder == null)
        //        {
        //            return BadRequest("Item or Order not found!");
        //        }

        //        var newOrderItem = new OrderItem
        //        {
        //            ItemId = orderItem.ItemId,
        //            Item = newItem,
        //            Quantity = orderItem.Quantity,
        //            OrderId = orderItem.OrderId,
        //            Order = newOrder
        //        };
        //        newOrderItem.OrderItemPrice = orderItem.Quantity * newOrderItem.Item.Price;

        //        _itemDbContext.OrderItems.Add(newOrderItem);
        //        await _itemDbContext.SaveChangesAsync();
        //        return RedirectToAction(nameof(Table));
        //    }
        //    catch
        //    {
        //        return BadRequest("OrderItem Creation failed.");
        //    }
        //}
    }
}
