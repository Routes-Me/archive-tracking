﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ArchiveTrackService.Models.DBModels
{
    public partial class archivetrackserviceContext : DbContext
    {
        public archivetrackserviceContext()
        {
        }

        public archivetrackserviceContext(DbContextOptions<archivetrackserviceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Coordinates> Coordinates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=nirmal;password=NirmalTheOne@123;database=archivetrackservice", x => x.ServerVersion("8.0.20-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coordinates>(entity =>
            {
                entity.HasKey(e => e.CoordinateId)
                    .HasName("PRIMARY");

                entity.ToTable("coordinates");

                entity.Property(e => e.CoordinateId)
                    .HasColumnName("coordinate_id")
                    .HasColumnType("varchar(40)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");

                entity.Property(e => e.DeviceId).HasColumnName("device_id");

                entity.Property(e => e.Latitude)
                    .HasColumnName("latitude")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.Longitude)
                    .HasColumnName("longitude")
                    .HasColumnType("decimal(10,0)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}