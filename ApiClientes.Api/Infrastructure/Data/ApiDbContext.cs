using ApiClientes.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiClientes.Api.Infrastructure.Data;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
            entity.Property(c => c.PasswordHash).IsRequired();
            entity.HasIndex(c => c.Email).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
