
namespace WebAppOsloMet.Models
{
    // Data model (data schema) for DB
    // User and Post are navigation properties.
    public class Upvote
    {
        public int UpvoteId { get; set; }
        public string Vote { get; set; } = string.Empty;
        public int UserId { get; set; }
        public virtual User User { get; set; } = default!;
        public int PostID { get; set; }
        public virtual Post Post { get; set; } = default!;     
    }
}
