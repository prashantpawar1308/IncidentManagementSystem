using IMS.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IMS.Data.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Incident> Incident { get; set; }

    public DbSet<DocumentMetadata> DocumentMetadata { get; set; }

    public DbSet<TEntity> Set<TEntity>() where TEntity : class => base.Set<TEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<DocumentMetadata>().HasKey(d => d.DocumentId);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Incident).Assembly);
    }
}
