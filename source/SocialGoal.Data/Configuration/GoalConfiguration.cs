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
    public class GoalConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.Property(g => g.GoalName).IsRequired().HasMaxLength(55);
            builder.Property(g => g.GoalType).IsRequired();
            builder.Property(g => g.GoalStatusId).IsRequired();
            builder.Property(g => g.StartDate).IsRequired();
            builder.Property(g => g.EndDate).IsRequired();
            builder.Property(g => g.CreatedDate).IsRequired();
            builder.Property(g => g.Desc).HasMaxLength(100);
        }
    }
}
