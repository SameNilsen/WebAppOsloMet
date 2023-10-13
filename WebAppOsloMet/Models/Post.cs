
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
        public string Title { get; set; } = string.Empty;
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
