using Microsoft.EntityFrameworkCore;
using Tasktile.Api.Domain;

namespace Tasktile.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>(e =>
        {
            e.ToTable("tasks");
            e.HasKey(t => t.Id);
            e.Property(t => t.OwnerId).HasMaxLength(128).IsRequired();
            e.Property(t => t.Title).HasMaxLength(500).IsRequired();
            e.Property(t => t.Notes).HasMaxLength(4000);
            e.HasIndex(t => new { t.OwnerId, t.IsDone });
        });
    }
}
