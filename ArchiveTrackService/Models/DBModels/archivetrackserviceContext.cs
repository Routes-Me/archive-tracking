using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ArchiveTrackService.Models.DBModels
{
    public partial class ArchiveTrackServiceContext : DbContext
    {
        public ArchiveTrackServiceContext()
        {
        }

        public ArchiveTrackServiceContext(DbContextOptions<ArchiveTrackServiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Coordinates> Coordinates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coordinates>(entity =>
            {
                entity.HasKey(e => e.CoordinateId).HasName("PRIMARY");

                entity.ToTable("coordinates");

                entity.Property(e => e.CoordinateId)
                    .HasColumnName("coordinate_id");

                entity.Property(e => e.ArchivedAt)
                    .HasColumnName("archived_at")
                    .HasColumnType("timestamp");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.DeviceId).HasColumnName("device_id");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasColumnType("decimal(10,8)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasColumnType("decimal(11,8)");

                entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
