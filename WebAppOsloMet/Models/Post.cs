
using System.ComponentModel.DataAnnotations;

namespace WebAppOsloMet.Models
{   
    // Data model (data schema) for DB
    // Navn under må starte med uppercase.
    // String Name har default Empty, sier at det må være en verdi.
    // string? betyr at det er nullable. Kan være null.
    // Virtual er knyttet til Lazy loading
    public class Post
    {
        public int PostID { get; set; }

        [RegularExpression(@"[0-9a-zA-Zæøå. \-]{2,20}", ErrorMessage = "The title must be numbers" +
            "or letters and between 2 to 20 char")]
        [Display(Name = "Post name")]
        public string Title { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public string? PostDate { get; set; }
        public int UserId { get; set; } = 1;
        public virtual User User { get; set; } = default!;
        public virtual List<Comment>? Comments { get; set; }
        public int UpvoteCount { get; set; } = 0;
        public virtual List<Upvote>? UserVotes { get; set; }
        public string SubForum { get; set; } = string.Empty;


    }
}
