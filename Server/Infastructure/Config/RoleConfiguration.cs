using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infastructure.Config;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole { Id = "352262c4-3b1c-45e2-ae39-47ee981cecb1", Name="Admin",NormalizedName="ADMIN"},
            new IdentityRole { Id = "7bd55849-babb-46b6-a1db-2c86233cae59", Name="Customer",NormalizedName="CUSTOMER"}
            );
    }
}
