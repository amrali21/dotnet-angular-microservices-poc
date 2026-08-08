using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using nextjs_backend_dashboard_service.Helpers;

namespace nextjs_backend_dashboard_service.Models
{
    public partial class nextjstestContext : DbContext
    {
        public nextjstestContext()
        {
        }

        public nextjstestContext(DbContextOptions<nextjstestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Revenue> Revenues { get; set; } = null!;
        public virtual DbSet<Kpi> Kpis { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Revenue>(entity =>
            {
                entity.HasKey(e => new { e.Month, e.Year });

                entity.ToTable("revenue");

                entity.Property(e => e.Month)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("month");

                entity.Property(e => e.Year).HasColumnName("year");

                entity.Property(e => e.Revenue1).HasColumnName("revenue");
            });

            // One row per named metric (Total Billed / Collected / Outstanding /
            // Customers), seeded ahead of time by db/seed data - dashbaord.sql.
            // KpiService updates kpivalue in place via atomic ExecuteUpdateAsync
            // calls keyed on ID/KpiName, not delete-then-insert.
            modelBuilder.Entity<Kpi>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.ToTable("kpis");

                entity.Property(e => e.ID).HasColumnName("ID");
                entity.Property(e => e.KpiName)
                    .HasMaxLength(200)
                    .HasColumnName("kpiname");
                entity.Property(e => e.KpiDesc)
                    .HasMaxLength(2000)
                    .HasColumnName("kpidesc");
                entity.Property(e => e.KpiValue).HasColumnName("kpivalue");
            });

            modelBuilder.Seed();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
