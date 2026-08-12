using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ledgerly_backend.Helpers;

namespace ledgerly_backend.Models
{
    public partial class ledgerlytestContext : DbContext
    {
        public ledgerlytestContext()
        {
        }

        public ledgerlytestContext(DbContextOptions<ledgerlytestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Invoice> Invoices { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoices");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("customer_id");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date");

                entity.Property(e => e.Status)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("status");
            });

            modelBuilder.Seed();

            OnModelCreatingPartial(modelBuilder);
        }

        void seedData(ModelBuilder modelBuilder)
        {

        }                       

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
