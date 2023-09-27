namespace WebAppOsloMet.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        // Navigation property
        public virtual List<Post>? Posts { get; set; }
    }
}
