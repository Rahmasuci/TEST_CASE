namespace ECommerce.Api.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int UserId { get; set; }
        public string? Status { get; set; }

        public List<OrderItem> Items { get; set; } = new();
    }
}