namespace WebAppOsloMet.Models
{   
    // Data model (data schema) for DB
    // Navn under må starte med uppercase.
    // String Name har default Empty, sier at det må være en verdi.
    // string? betyr at det er nullable. Kan være null.
    // Virtual er knyttet til Lazy loading
    public class Item
    {
        public int ItemID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        //  Navigation Property
        public virtual List<OrderItem>? OrderItems { get; set; }
    }
}
