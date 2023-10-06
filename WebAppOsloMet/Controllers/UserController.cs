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

        public UserController(PostDbContext postDbContext)
        {
            _postDbContext = postDbContext;
        }

        public async Task<IActionResult> MyPosts(string id)
        {
            //       GET USER FROM IDENTITYUSER
            var user = _postDbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == id).Result;
            if (user.Posts == null)
            {
                return BadRequest("No posts...");
            }
            List<Post> posts = user.Posts;
            return View("Table", posts);
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
