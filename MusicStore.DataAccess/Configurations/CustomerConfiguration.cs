using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicStore.Entities;

namespace MusicStore.DataAccess.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(p => p.Email)
            .HasMaxLength(200)
            //.HasColumnType("varchar(200)");
            .IsRequired()
            .IsUnicode(false);

        builder.Property(p => p.FullName)
            .HasMaxLength(200);

        // Crear relacion con Fluent API
        //builder.HasMany(p => p.Sales)
        //    .WithOne(p => p.Customer)
        //    .HasForeignKey(p => p.CustomerFk)
        //    .OnDelete(DeleteBehavior.Restrict);
    }
}