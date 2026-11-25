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
    public class GoalStatusConfiguration : IEntityTypeConfiguration<GoalStatus>
    {
        public void Configure(EntityTypeBuilder<GoalStatus> builder)
        {
            builder.Property(g => g.GoalStatusType).HasMaxLength(50);
        }
    }
}
