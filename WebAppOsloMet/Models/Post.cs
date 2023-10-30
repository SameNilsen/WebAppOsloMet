
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebAppOsloMet.Models
{
    // Data model (data schema) for DB
    // User, Comments and UserVotes are navigation properties.
    public class Post
    {
        public int PostID { get; set; }

        [RegularExpression(@"[0-9a-zA-Zæøå. \-]{2,80}", ErrorMessage = "The title must contain numbers " +
            "or letters, and must be between 2 and 80 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Description")]
        [Required]
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public string? PostDate { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; } = default!;
        public virtual List<Comment>? Comments { get; set; }
        public int UpvoteCount { get; set; } = 0;
        public virtual List<Upvote>? UserVotes { get; set; }
        [Display(Name = "Subforum")]
        public string SubForum { get; set; } = string.Empty;
    }
}
