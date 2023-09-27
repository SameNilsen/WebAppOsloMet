﻿namespace WebAppOsloMet.Models
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
        public string OrderDate { get; set; } = string.Empty;
        public int UserId { get; set; }
        public virtual User User { get; set; } = default!;

    }
}