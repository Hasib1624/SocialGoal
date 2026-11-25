using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Models
{
    public class SocialGoalEntities : IdentityDbContext<ApplicationUser>
    {
        public SocialGoalEntities(DbContextOptions<SocialGoalEntities> options)
            : base(options)
        {
        }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Focus> Focuses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<Metric> Metrics { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<SecurityToken> SecurityTokens { get; set; }
        public DbSet<Support> Support { get; set; }
        public DbSet<GroupInvitation> GroupInvitation { get; set; }
        public DbSet<SupportInvitation> SupportInvitation { get; set; }
        public DbSet<GroupGoal> GroupGoal { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<GoalStatus> GoalStatus { get; set; }
        public DbSet<GroupUpdate> GroupUpdate { get; set; }
        public DbSet<GroupComment> GroupComment { get; set; }
        public DbSet<GroupRequest> GroupRequests { get; set; }
        public DbSet<FollowRequest> FollowRequest { get; set; }
        public DbSet<FollowUser> FollowUser { get; set; }
        public DbSet<GroupUpdateUser> GroupUpdateUsers { get; set; }
        public DbSet<GroupCommentUser> GroupCommentUsers { get; set; }
        public DbSet<CommentUser> CommentUsers { get; set; }
        public DbSet<UpdateSupport> UpdateSupports { get; set; }
        public DbSet<GroupUpdateSupport> GroupUpdateSupports { get; set; }

        public virtual void Commit()
        {
            SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SocialGoalEntities).Assembly);

            // Lookup rows, same ids as the original's seed order.
            string[] metrics = { "%", "$", "$ M", "Rs", "Hours", "Km", "Kg", "Years" };
            modelBuilder.Entity<Metric>().HasData(metrics.Select((type, i) => new Metric { MetricId = i + 1, Type = type }));
            string[] statuses = { "In Progress", "On Hold", "Completed" };
            modelBuilder.Entity<GoalStatus>().HasData(statuses.Select((type, i) => new GoalStatus { GoalStatusId = i + 1, GoalStatusType = type }));
        }
    }
}
