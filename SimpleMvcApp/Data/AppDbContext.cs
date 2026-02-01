using Microsoft.EntityFrameworkCore;
using SimpleMvcApp.Models;

namespace SimpleMvcApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Product> Products => Set<Product>();
}