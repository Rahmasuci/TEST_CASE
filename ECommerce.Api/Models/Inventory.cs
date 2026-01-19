namespace ECommerce.Api.Models
{
    public class Inventory
    {
        public int InventoryID { get; set; }
        public string Sku { get; set; } = default!;
        public int ActualQty { get; set; }
        public int ReservedQty { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}