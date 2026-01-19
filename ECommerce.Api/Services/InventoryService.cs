
using ECommerce.Api.Data;
using ECommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services
{
    public class InventoryService
    {
        private readonly AppDbContext _context;

        public InventoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Inventory> GetInventoryAscy(string sku)
        {
            try
            {
                var inv = await _context.Inventories.FirstOrDefaultAsync(x => x.Sku == sku);

                if (inv == null)
                {
                    var message = "Inventory dengan Sku " + sku + " tidak ditemukan";
                    throw new Exception(message);
                }

                return inv;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}