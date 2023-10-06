using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppOsloMet.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual List<Comment>? Comments { get; set; }
        [ForeignKey("IdentityUser")]
        public string? IdentityUserId { get; set; }
        public virtual IdentityUser? IdentityUser { get; set; }
        // Navigation property
        public virtual List<Post>? Posts { get; set; }
    }

    //public class ApplicationUser : IdentityUser
    //{
    //    public virtual User User { get; set; }
    //}
}
