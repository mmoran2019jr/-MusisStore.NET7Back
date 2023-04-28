using Microsoft.EntityFrameworkCore;
using MusicStore.Entities.Infos;
using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MusicStore.DataAccess;

public class MusicStoreDbContext : IdentityDbContext<MusicStoreUserIdentity>
{
    public MusicStoreDbContext(DbContextOptions<MusicStoreDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


        // Esto solo se utiliza cuando se usa EF Core
        //modelBuilder.Entity<ReportInfo>()
        //    .HasNoKey(); // This is necessary to avoid the error: "The entity type 'ReportInfo' requires a primary key to be defined.
        //                 // "

        //modelBuilder.Entity<ReportInfo>()
        //    .Property(p => p.Total)
        //    .HasPrecision(11, 2);
    }
}