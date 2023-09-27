namespace WebAppOsloMet.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderDate { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        //  Navigation property
        public virtual Customer Customer { get; set; } = default!;
        //  Navigation property
        public virtual List<OrderItem>? OrderItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
