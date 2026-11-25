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
    public class FocusConfiguration : IEntityTypeConfiguration<Focus>
    {
        public void Configure(EntityTypeBuilder<Focus> builder)
        {
            builder.Property(f => f.FocusName).HasMaxLength(50);
            builder.Property(f => f.Description).HasMaxLength(100);
        }
    }
}
