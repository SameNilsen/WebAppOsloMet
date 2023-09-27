namespace WebAppOsloMet.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int ItemId { get; set; }
        //  Navigation Property
        public virtual Item Item { get; set; } = default!;
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        //  Navigation property
        public virtual Order Order { get; set; } = default!;
        public decimal OrderItemPrice { get; set; }
    }
}
