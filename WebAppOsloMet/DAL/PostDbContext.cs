using Microsoft.EntityFrameworkCore;
using WebAppOsloMet.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebAppOsloMet.DAL
{
    public class PostDbContext : IdentityDbContext // Change from DbContext to IdentityDbContext for authorize/authenticate
    {
        public PostDbContext(DbContextOptions<PostDbContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Upvote> Upvotes { get; set; }
        //public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

    }
}
