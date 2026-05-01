using Microsoft.EntityFrameworkCore;
using MydeApi.Models;

namespace MydeApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Proposal> Proposals { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Índice único: CPF único por tenant
        modelBuilder.Entity<Client>()
            .HasIndex(c => new { c.TenantId, c.Cpf })
            .IsUnique();
        
        // Índice único: Email único por tenant
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.TenantId, u.Email })
            .IsUnique();
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // CORREÇÃO: Usar UTC em todos os DateTimes
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();
    }
}

// Converter que garante UTC
public class UtcDateTimeConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() 
        : base(
            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}
