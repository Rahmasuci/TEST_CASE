namespace ECommerce.Api.Models
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public string Sku { get; set; } = default!;
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}