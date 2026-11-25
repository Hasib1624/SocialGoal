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
    public class SupportConfiguration : IEntityTypeConfiguration<Support>
    {
        public void Configure(EntityTypeBuilder<Support> builder)
        {
            builder.Property(s => s.SupportedDate).IsRequired();
            builder.Property(s => s.GoalId).IsRequired();
            builder.Property(s => s.UserId);
    }
    }
}
