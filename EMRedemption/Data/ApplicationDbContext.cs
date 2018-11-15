using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EMRedemption.Models;
using EMRedemption.Entities;
using EMRedemption.Models.RewardViewModels;
using EMRedemption.Models.RedemptionViewModels;
using EMRedemption.Models.RewardTypeViewModels;

namespace EMRedemption.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<RewardType> RewardTypes { get; set; }
        public DbSet<Redemption> Redemptions { get; set; }
        public DbSet<RedemptionItem> RedemptionItems { get; set; }
        public DbSet<AppLog> AppLogs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
