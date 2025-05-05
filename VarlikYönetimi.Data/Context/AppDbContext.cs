using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Data.Context
{
    public class AppDbContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<AdvanceRequest> AdvanceRequests { get; set; }
        public DbSet<AdvanceRequestProject> AdvanceRequestProjects { get; set; }
        public DbSet<ApprovalProcess> ApprovalProcesses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<LegalAction> LegalActions { get; set; }
        public DbSet<ApprovalSettings> ApprovalSettings { get; set; }
        public DbSet<PasswordHistory> PasswordHistory { get; set; }
        public DbSet<SecurityEvent> SecurityEvents { get; set; }
        public DbSet<RevokedToken> RevokedTokens { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AdvanceLimit> AdvanceLimits { get; set; }
        public DbSet<GeneralAdvanceLimit> GeneralAdvanceLimits { get; set; }
        public DbSet<DepartmentApprovalLimit> DepartmentApprovalLimits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdvanceRequestProject>()
                .HasOne(arp => arp.AdvanceRequest)
                .WithMany(ar => ar.AdvanceRequestProjects)
                .HasForeignKey(arp => arp.AdvanceRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdvanceRequestProject>()
                .HasOne(arp => arp.Project)
                .WithMany(p => p.AdvanceRequestProjects)
                .HasForeignKey(arp => arp.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApprovalProcess>()
                .HasOne(ap => ap.AdvanceRequest)
                .WithMany(ar => ar.Approvals)
                .HasForeignKey(ap => ap.AdvanceRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApprovalProcess>()
                .HasOne(ap => ap.ApproverUser)
                .WithMany()
                .HasForeignKey(ap => ap.ApproverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.AdvanceRequest)
                .WithMany(ar => ar.Payments)
                .HasForeignKey(p => p.AdvanceRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.EnteredByUser)
                .WithMany()
                .HasForeignKey(p => p.EnteredByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.DeliveredByUser)
                .WithMany()
                .HasForeignKey(p => p.DeliveredByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            var strategy = Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await Database.BeginTransactionAsync();
                try
                {
                    await operation();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> operation)
        {
            var strategy = Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await Database.BeginTransactionAsync();
                try
                {
                    var result = await operation();
                    await transaction.CommitAsync();
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}