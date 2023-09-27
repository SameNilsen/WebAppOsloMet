namespace WebAppOsloMet.Models
{
    public class Customer
    {
        public int CustomerId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        // Navigation property
        public virtual List<Order>? Orders { get; set; }
    }
}
