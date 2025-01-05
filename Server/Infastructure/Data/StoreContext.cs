using Core.Models;
using Infastructure.Config;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Data;

public class StoreContext(DbContextOptions opt) : IdentityDbContext<AppUser>(opt)
{
   public DbSet<Product> Products { get; set; }
   public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);

    }
}
