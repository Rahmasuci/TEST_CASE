using Microsoft.AspNetCore.Mvc;
using SimpleMvcApp.Data;
using SimpleMvcApp.Models;
using SimpleMvcApp.ViewModel;

namespace SimpleMvcApp.Controllers
{
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly AppDbContext _db;


        public ProductController(AppDbContext db, ILogger<ProductController> logger)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet("")]
        public IActionResult Index(string? keyword, int page = 1, int pageSize = 10)
        {

            var query = _db.Products.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(p => p.Name.Contains(keyword));

            var totalItems = query.Count();

            var products = query
                .OrderByDescending(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new ProductListVm
            {
                Products = products,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(model);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            _db.Products.Add(product);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost("Edit/{id}")]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(product);


            _db.Products.Update(product);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            return View(product);
        }


        [HttpPost("Delete/{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            _db.Products.Remove(product);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // [HttpGet]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}