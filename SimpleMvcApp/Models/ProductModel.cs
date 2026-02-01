using System.ComponentModel.DataAnnotations;

namespace SimpleMvcApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [Range(1, 1_000_000)]
        public decimal Price { get; set; }
    }
}