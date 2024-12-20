using Core.Models;
using Infastructure.Config;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Data;

public class StoreContext(DbContextOptions opt) : DbContext(opt)
{
   public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);

    }
}
