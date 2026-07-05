using gestao_sinistros_api.Application.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<Cliente> Clientes { get; set; }

    public DbSet<Apolice> Apolices { get; set; }

    public DbSet<Sinistro> Sinistros { get; set; }

    public DbSet<HistoricoSinistro> HistoricoSinistros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Cliente>()
            .HasQueryFilter(c => c.DeletedAt == null);

        modelBuilder.Entity<Apolice>()
            .HasQueryFilter(a => a.DeletedAt == null);

        modelBuilder.Entity<Sinistro>()
            .HasQueryFilter(s => s.DeletedAt == null);

        modelBuilder.Entity<HistoricoSinistro>()
            .HasQueryFilter(h => h.DeletedAt == null);
    }
}
