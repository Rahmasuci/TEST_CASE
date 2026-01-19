namespace ECommerce.Api.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}