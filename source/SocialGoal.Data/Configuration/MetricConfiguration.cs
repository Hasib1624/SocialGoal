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
    public class MetricConfiguration : IEntityTypeConfiguration<Metric>
    {
        public void Configure(EntityTypeBuilder<Metric> builder)
        {
            builder.Property(m => m.Type);
        }
    }
}
