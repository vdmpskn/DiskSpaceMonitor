using Microsoft.EntityFrameworkCore;
using DiskSpaceMonitor.Models;

namespace DiskSpaceMonitor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<FileMetadata> Files { get; set; }
        public DbSet<Server> Servers { get; set; }

        public DbSet<Folder> Folders { get; set; }

        public DbSet<Quota> Quotas { get; set; }

        public DbSet<Disk> Disks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування FileMetadata
            modelBuilder.Entity<FileMetadata>(entity =>
            {
                entity.Property(f => f.FileName).IsRequired().HasMaxLength(255);
                entity.Property(f => f.FilePath).IsRequired();
                entity.Property(f => f.FileType).HasMaxLength(10);

                // Убираем каскадное удаление для ServerId
                entity.HasOne(f => f.Server)
                      .WithMany(s => s.Files)
                      .HasForeignKey(f => f.ServerId)
                      .OnDelete(DeleteBehavior.Restrict);  // Убираем каскадное удаление

                // Убираем каскадное удаление для DiskId, ставим NULL при удалении диска
                entity.HasOne(f => f.Disk)
                      .WithMany(d => d.Files)
                      .HasForeignKey(f => f.DiskId)
                      .OnDelete(DeleteBehavior.SetNull);  // Ставим NULL вместо каскада
            });

            // Налаштування AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(a => a.Action).IsRequired().HasMaxLength(50);

                // Каскадное удаление для файлового лога
                entity.HasOne(a => a.File)
                      .WithMany(f => f.AuditLogs)
                      .HasForeignKey(a => a.FileId)
                      .OnDelete(DeleteBehavior.Cascade);  // Логично оставить каскадное удаление для аудита
            });

            // Налаштування Server
            modelBuilder.Entity<Server>(entity =>
            {
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.IPAddress).IsRequired().HasMaxLength(50);

                // Привязываем сервер к дискам с запретом на каскадное удаление
                entity.HasMany(s => s.Disks)
                      .WithOne(d => d.Server)
                      .HasForeignKey(d => d.ServerId)
                      .OnDelete(DeleteBehavior.Restrict);  // Избегаем каскадного удаления
            });

            // Налаштування Disk
            modelBuilder.Entity<Disk>(entity =>
            {
                entity.Property(d => d.Name).IsRequired().HasMaxLength(10);

                // Привязываем диски к серверу с запретом на каскадное удаление
                entity.HasOne(d => d.Server)
                      .WithMany(s => s.Disks)
                      .HasForeignKey(d => d.ServerId)
                      .OnDelete(DeleteBehavior.Restrict);  // Избегаем каскадного удаления
            });
        }


    }
}
