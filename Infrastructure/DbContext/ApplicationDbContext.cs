using Domain.Enitities;
using Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configure Appointment entity
            
            builder.Entity<Appointment>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId);

            // Relación RefreshToken -> ApplicationUser
            builder.Entity<RefreshToken>()
                .HasOne<ApplicationUser>()
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId);

            // Relación AuditLog -> ApplicationUser
            builder.Entity<AuditLog>()
                .HasOne<ApplicationUser>()
                .WithMany(u =>u.AuditLogs)
                .HasForeignKey(l => l.UserId);

        }
    }
    }
