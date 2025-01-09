﻿using Core.Models;
using Core.Models.OrderAggregate;
using Infastructure.Config;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infastructure.Data;

public class StoreContext(DbContextOptions opt) : IdentityDbContext<AppUser>(opt)
{
   public DbSet<Product> Products { get; set; }
   public DbSet<Address> Addresses { get; set; }
   public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
   public DbSet<Order> Orders { get; set; }
   public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);

    }
}
