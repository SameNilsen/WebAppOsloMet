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

        //  "Users" has a warning because the name User is also (unfortunatly) used by 
        //    the premade IdentityUser from Microsoft.AspNetCore.Identity. This is 
        //     unfortunatly reccuring thing throughout.
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Upvote> Upvotes { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

    }
}
