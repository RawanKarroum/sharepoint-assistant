using Microsoft.EntityFrameworkCore;
using SPRagAPI.Models;

namespace SPRagAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<DocumentEntity> Documents => Set<DocumentEntity>();
    public DbSet<DocumentChunk> Chunks => Set<DocumentChunk>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentEntity>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Title).HasMaxLength(500);
            entity.Property(d => d.WebUrl).HasMaxLength(2000);
            entity.Property(d => d.ExtractedText).HasColumnType("nvarchar(max)");
        });

        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Content).HasColumnType("nvarchar(max)");
            entity.Property(c => c.DocumentTitle).HasMaxLength(500);
            entity.Property(c => c.DocumentUrl).HasMaxLength(2000);

            entity.HasIndex(c => c.DocumentId);

            entity.HasOne<DocumentEntity>()
                .WithMany(d => d.Chunks)
                .HasForeignKey(c => c.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
