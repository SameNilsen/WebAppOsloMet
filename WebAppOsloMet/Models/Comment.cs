using System.ComponentModel.DataAnnotations;

namespace WebAppOsloMet.Models
{
    // Data model (data schema) for DB
    // User and Post are navigation properties.
    
    public class Comment
    {
        public int CommentID { get; set; }

        [StringLength(200)]
        public string CommentText { get; set; } = string.Empty;
        public string? PostDate { get; set; }
        public int UserId { get; set; } = 1;
        public virtual User User { get; set; } = default!;
        public int PostID { get; set; }
        public virtual Post Post { get; set; } = default!;


    }
}
