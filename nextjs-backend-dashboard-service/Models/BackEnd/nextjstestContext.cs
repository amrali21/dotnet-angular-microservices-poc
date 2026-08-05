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
                entity.HasKey(e => new { e.Month, e.Revenue1 });

                entity.ToTable("revenue");

                entity.HasIndex(e => e.Month, "UQ__revenue__0DD75472E02AC0B3")
                    .IsUnique();

                entity.Property(e => e.Month)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("month");

                entity.Property(e => e.Revenue1).HasColumnName("revenue");
            });

            // kpis has no primary key by design — it's a single-row snapshot,
            // replaced wholesale (delete-all-then-insert) by KPI/updateKpis.
            modelBuilder.Entity<Kpi>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("kpis");

                entity.Property(e => e.TotalBilled).HasColumnName("totalbilled");
                entity.Property(e => e.Collected).HasColumnName("collected");
                entity.Property(e => e.Outstanding).HasColumnName("outstanding");
                entity.Property(e => e.Customers).HasColumnName("customers");
            });

            modelBuilder.Seed();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
