namespace ECommerce.Api.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int OrderID { get; set; }
        public string PaymentExternalID { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset PaidAt { get; set; }
        public string Status { get; set; }

        public Order Order { get; set; }
    }
}