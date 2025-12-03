using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FindMe.DAL.Models;

public partial class FindMeDbContext : DbContext
{
    public FindMeDbContext()
    {
    }

    public FindMeDbContext(DbContextOptions<FindMeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Campus> Campuses { get; set; }

    public virtual DbSet<Claim> Claims { get; set; }

    public virtual DbSet<ClaimVerification> ClaimVerifications { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemHistory> ItemHistories { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enum conversions
        modelBuilder.Entity<Item>()
            .Property(e => e.ReportType)
            .HasConversion(
                v => v.ToString(),
                v => (ReportType)Enum.Parse(typeof(ReportType), v));

        modelBuilder.Entity<Item>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ItemStatus)Enum.Parse(typeof(ItemStatus), v));

        modelBuilder.Entity<Claim>()
            .Property(e => e.ClaimStatus)
            .HasConversion(
                v => v.ToString(),
                v => (ClaimStatus)Enum.Parse(typeof(ClaimStatus), v));

        modelBuilder.Entity<ItemHistory>()
            .Property(e => e.FromStatus)
            .HasConversion(
                v => v.HasValue ? v.Value.ToString() : null,
                v => string.IsNullOrEmpty(v) ? null : (ItemStatus?)Enum.Parse(typeof(ItemStatus), v));

        modelBuilder.Entity<ItemHistory>()
            .Property(e => e.ToStatus)
            .HasConversion(
                v => v.ToString(),
                v => (ItemStatus)Enum.Parse(typeof(ItemStatus), v));

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A6360EFA65");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__A9D105348408C51C").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<Campus>(entity =>
        {
            entity.HasKey(e => e.CampusId).HasName("PK__Campus__FD598DD6534EF797");

            entity.ToTable("Campus");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(e => e.ClaimId).HasName("PK__Claim__EF2E139B3DFC9156");

            entity.ToTable("Claim");

            entity.HasIndex(e => e.ItemId, "IX_Claim_Item");

            entity.HasIndex(e => e.ClaimStatus, "IX_Claim_Status");

            entity.Property(e => e.ClaimStatus)
                .HasMaxLength(50);
            entity.Property(e => e.ClaimSubmittedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ClaimerEmail).HasMaxLength(255);
            entity.Property(e => e.ClaimerName).HasMaxLength(200);
            entity.Property(e => e.ClaimerPhone).HasMaxLength(50);

            entity.HasOne(d => d.AssignedOfficer).WithMany(p => p.ClaimAssignedOfficers)
                .HasForeignKey(d => d.AssignedOfficerId)
                .HasConstraintName("FK_Claim_AssignedOfficer");

            entity.HasOne(d => d.ClaimerAccount).WithMany(p => p.ClaimClaimerAccounts)
                .HasForeignKey(d => d.ClaimerAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Claim_Claimer");

            entity.HasOne(d => d.Item).WithMany(p => p.Claims)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Claim_Item");
        });

        modelBuilder.Entity<ClaimVerification>(entity =>
        {
            entity.HasKey(e => e.VerificationId).HasName("PK__ClaimVer__306D490779688599");

            entity.ToTable("ClaimVerification");

            entity.Property(e => e.ActionAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ClaimMediaPath).HasMaxLength(1000);

            entity.HasOne(d => d.Claim).WithMany(p => p.ClaimVerifications)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClaimVerification_Claim");

            entity.HasOne(d => d.Officer).WithMany(p => p.ClaimVerifications)
                .HasForeignKey(d => d.OfficerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClaimVerification_Officer");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Item__727E838BE848B213");

            entity.ToTable("Item");

            entity.HasIndex(e => e.CampusId, "IX_Item_Campus");

            entity.HasIndex(e => e.Status, "IX_Item_Status");

            entity.Property(e => e.Identifiers).HasMaxLength(500);
            entity.Property(e => e.ImagePath).HasMaxLength(1000);
            entity.Property(e => e.ReportType).HasMaxLength(20);
            entity.Property(e => e.ReportedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(50);
            entity.Property(e => e.StorageCode).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(250);

            entity.HasOne(d => d.Campus).WithMany(p => p.Items)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Item_Campus");

            entity.HasOne(d => d.Location).WithMany(p => p.Items)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_Item_Location");

            entity.HasOne(d => d.ReporterAccount).WithMany(p => p.Items)
                .HasForeignKey(d => d.ReporterAccountId)
                .HasConstraintName("FK_Item_Reporter");
        });

        modelBuilder.Entity<ItemHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__ItemHist__4D7B4ABD77C2D5C8");

            entity.ToTable("ItemHistory");

            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FromStatus).HasMaxLength(50);
            entity.Property(e => e.ToStatus).HasMaxLength(50);

            entity.HasOne(d => d.ChangedByAccount).WithMany(p => p.ItemHistories)
                .HasForeignKey(d => d.ChangedByAccountId)
                .HasConstraintName("FK_ItemHistory_Account");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemHistories)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ItemHistory_Item");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__E7FEA497A12519AF");

            entity.ToTable("Location");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Campus).WithMany(p => p.Locations)
                .HasForeignKey(d => d.CampusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Location_Campus");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
