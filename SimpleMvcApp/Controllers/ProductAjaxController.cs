
using Microsoft.AspNetCore.Mvc;
using SimpleMvcApp.Data;
using SimpleMvcApp.Models;
using SimpleMvcApp.ViewModel;

namespace SimpleMvcApp.Controllers
{
    [Route("[controller]")]
    public class ProductAjaxController : Controller
    {
        private readonly ILogger<ProductAjaxController> _logger;
        private readonly AppDbContext _db;


        public ProductAjaxController(AppDbContext db, ILogger<ProductAjaxController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("LoadTable")]
        public IActionResult LoadTable(string? keyword, int page = 1, int pageSize = 10)
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

            return PartialView("_Table", model);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return PartialView("_FormCreate");
        }



        [HttpPost("Create")]
        public IActionResult Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            _db.Products.Add(product);
            _db.SaveChanges();

            return Ok();
        }


        [HttpGet("Edit")]
        public IActionResult Edit(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            return PartialView("_FormEdit", product);
        }

        [HttpPost("Update")]
        public IActionResult Update(Product product)
        {
            _db.Products.Update(product);
            _db.SaveChanges();

            return Ok();
        }

        [HttpPost("Delete")]
        public IActionResult Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return NotFound();

            _db.Products.Remove(product);
            _db.SaveChanges();

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}