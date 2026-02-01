using SimpleMvcApp.Models;

namespace SimpleMvcApp.ViewModel
{
    public class ProductListVm
    {
        public List<Product> Products { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalItems / PageSize);
    }
}