namespace WebAppOsloMet.Models
{
    // Data model (data schema) for DB
    // Navn under må starte med uppercase.
    // String Name har default Empty, sier at det må være en verdi.
    // string? betyr at det er nullable. Kan være null.
    // Virtual er knyttet til Lazy loading
    public class Comment
    {
        public int CommentID { get; set; }
        public string CommentText { get; set; } = string.Empty;
        //public string? ImageUrl { get; set; }      //  Kommentere bilder i fremtiden???
        public string? PostDate { get; set; }
        public int UserId { get; set; } = 1;
        public virtual User User { get; set; } = default!;
        public int PostID { get; set; }
        public virtual Post Post { get; set; } = default!;


    }
}
