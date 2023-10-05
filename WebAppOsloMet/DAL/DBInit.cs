using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WebAppOsloMet.Models;

namespace WebAppOsloMet.DAL
{
    public static class DBInit
    {
        public static void Seed(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            //ItemDbContext context = serviceScope.ServiceProvider.GetRequiredService<ItemDbContext>();
            PostDbContext context = serviceScope.ServiceProvider.GetRequiredService<PostDbContext>();

            //context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Name = "ChoiceCold256"
                    },
                    new User
                    {
                        Name = "RevolutionaryBit660"
                    }
                };
                context.AddRange(users);
                context.SaveChanges();
            }

            if (!context.Posts.Any())
            {
                var posts = new List<Post>
                {
                    new Post
                    {
                        Title = "LEGO® Star Wars™: The Skywalker Saga",
                        Text = "What do you think about this game?" +
                        "I have about 50 hours into the game now, but I would like to hear your opinions.",
                        ImageUrl = "/images/legostarwars.jpg",
                        UserId = 1,
                        PostDate = DateTime.Today.ToString()
                    },
                    new Post
                    {
                        Title = "Second Post!!",
                        Text = "Epic post about shit",
                        ImageUrl = "/images/ribs.jpg",
                        UserId = 2,
                        PostDate = DateTime.Today.ToString()
                    },
                    new Post
                    {
                        Title = "Third Post!!",
                        Text = "Epic post about stuff",
                        ImageUrl = "/images/coke.jpg",
                        UserId = 1,
                        PostDate = DateTime.Today.ToString()
                    },
                };
                context.AddRange(posts);
                context.SaveChanges();
            }


            //if (!context.Orders.Any())
            //{
            //    var orders = new List<Order>
            //    {
            //        new Order {OrderDate = DateTime.Today.ToString(), CustomerId = 1},
            //        new Order {OrderDate = DateTime.Today.ToString(), CustomerId = 2},
            //    };
            //    context.AddRange(orders);
            //    context.SaveChanges();
            //}

            //if (!context.OrderItems.Any())
            //{
            //    var orderItems = new List<OrderItem>
            //    {
            //        new OrderItem { ItemId = 1, Quantity = 2, OrderId = 1},
            //        new OrderItem { ItemId = 2, Quantity = 1, OrderId = 1},
            //        new OrderItem { ItemId = 3, Quantity = 4, OrderId = 2},
            //    };
            //    foreach (var orderItem in orderItems)
            //    {
            //        var item = context.Items.Find(orderItem.ItemId);
            //        orderItem.OrderItemPrice = orderItem.Quantity * item?.Price ?? 0;
            //    }
            //    context.AddRange(orderItems);
            //    context.SaveChanges();
            //}

            //var ordersToUpdate = context.Orders.Include(o => o.OrderItems);
            //foreach (var order in ordersToUpdate)
            //{
            //    order.TotalPrice = order.OrderItems?.Sum(oi => oi.OrderItemPrice) ?? 0;
            //}
            //context.SaveChanges();
        }
    }
}
