using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ClientesDbContext : DbContext
{
    public DbSet<Cliente> Clientes => Set<Cliente>();

    public ClientesDbContext(DbContextOptions<ClientesDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(150);
            entity.Property(c => c.PasswordHash).IsRequired();
            entity.Property(c => c.FechaRegistro).IsRequired();
            entity.HasIndex(c => c.Email).IsUnique();
        });
    }
}
