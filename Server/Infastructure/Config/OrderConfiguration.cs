using Core.Models.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infastructure.Config;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(x => x.ShippingAddress, o => o.WithOwner());
        builder.OwnsOne(x => x.PaymentSummary, o => o.WithOwner());
        builder.Property(x => x.OrderStatus).HasConversion(
            o => o.ToString(),
            o => (OrderStatus)Enum.Parse(typeof(OrderStatus),o));

        builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
        builder.Property(o=>o.Discount).HasColumnType("decimal(18,2)");

        builder.HasMany(x=>x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.OrderDate).HasConversion(
            d=>d.ToUniversalTime(),
            d=>DateTime.SpecifyKind(d,DateTimeKind.Utc)
            );

    }
}
