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
        public virtual IdentityUser? IdentityUser { get; set; }  //  Link to the IdentityUser.
        // Navigation property
        public virtual List<Post>? Posts { get; set; }
        public virtual List<Upvote>? UserVotes { get; set; }
        public int Credebility { get; set; }
    }
}
