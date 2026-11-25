using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialGoal.Model.Models;

namespace SocialGoal.Data.Configuration
{
    public class GroupGoalConfiguration : IEntityTypeConfiguration<GroupGoal>
    {
        public void Configure(EntityTypeBuilder<GroupGoal> builder)
        {
            builder.Property(g => g.GroupId).IsRequired();
            builder.Property(g => g.GoalName).HasMaxLength(50);
            builder.Property(g => g.Description).HasMaxLength(100);
            builder.Property(g => g.StartDate).IsRequired();
            builder.Property(g => g.EndDate).IsRequired();
            builder.Property(g => g.GoalStatusId).IsRequired();
            builder.Property(g => g.GroupUserId).IsRequired();
        }
    }
}
