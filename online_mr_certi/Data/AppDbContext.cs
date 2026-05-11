using Microsoft.EntityFrameworkCore;
using online_mr_certi.Models;

namespace online_mr_certi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<MarriageApplication> MarriageApplications => Set<MarriageApplication>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<MarriageWitness> MarriageWitnesses => Set<MarriageWitness>();
    public DbSet<Fee> Fees => Set<Fee>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AdminNotificationReadState> AdminNotificationReadStates => Set<AdminNotificationReadState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(u => u.RoleEntity)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();

        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Name)
            .IsUnique();

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MarriageApplication>()
            .HasOne(a => a.User)
            .WithMany(u => u.MarriageApplications)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Document>()
            .HasOne(d => d.Application)
            .WithMany(a => a.Documents)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Certificate>()
            .HasOne(c => c.Application)
            .WithOne(a => a.Certificate)
            .HasForeignKey<Certificate>(c => c.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MarriageWitness>()
            .HasOne(w => w.Application)
            .WithMany(a => a.Witnesses)
            .HasForeignKey(w => w.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MarriageWitness>()
            .HasIndex(w => new { w.ApplicationId, w.SortOrder })
            .IsUnique();

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Application)
            .WithOne(a => a.Payment)
            .HasForeignKey<Payment>(p => p.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Fee)
            .WithMany(f => f.Payments)
            .HasForeignKey(p => p.FeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.ApplicationId)
            .IsUnique();

        modelBuilder.Entity<AdminNotificationReadState>()
            .HasKey(n => n.UserId);

        modelBuilder.Entity<AdminNotificationReadState>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
