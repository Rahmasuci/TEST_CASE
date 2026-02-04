using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using SimpleMvcApp.Models;
using System.Data;
using System.Security.Cryptography.Pkcs;
using SimpleMvcApp.ViewModel;

namespace SimpleMvcApp.Controllers
{
    [Route("[controller]")]
    public class ProductSPController : Controller
    {
        private readonly ILogger<ProductSPController> _logger;

        private readonly string _connectionString;


        public ProductSPController(ILogger<ProductSPController> logger, IConfiguration config)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, int page = 1, int pageSize = 10)
        {
            using var conn = new SqlConnection(_connectionString);

            // var result = await conn.QueryAsync<Product>(
            //     "sp_GetProducts",

            //     commandType: CommandType.StoredProcedure);

            // var data = result.ToList();

            var result = await conn.QueryMultipleAsync(
                "sp_GetProducts",
                new
                {
                    keyword = keyword,
                    page = page,
                    pageSize = pageSize
                },
                commandType: CommandType.StoredProcedure
            );

            var products = result.Read<Product>().ToList();
            var totalRows = result.Read<int>().First();

            var model = new ProductListVm
            {
                Products = products,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalRows
            };

            return View(model);
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}