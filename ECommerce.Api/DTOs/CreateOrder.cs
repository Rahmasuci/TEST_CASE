using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace ECommerce.Api.DTOs
{
    public class CreateOrder
    {
        public int UserID { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string Sku { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
    }
}