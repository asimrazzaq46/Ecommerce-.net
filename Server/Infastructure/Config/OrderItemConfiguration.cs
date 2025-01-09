using Core.Models.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infastructure.Config;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(o => o.ItemOrdered,o=>o.WithOwner());
        builder.Property(x=>x.Price).HasColumnType("decimal(18,2)");
    }
}
