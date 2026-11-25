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
    public class GroupUpdateSupportConfiguration : IEntityTypeConfiguration<GroupUpdateSupport>
    {
        public void Configure(EntityTypeBuilder<GroupUpdateSupport> builder)
        {
            builder.Property(g => g.GroupUpdateId).IsRequired();
            builder.Property(g => g.GroupUserId).IsRequired();
            builder.Property(g => g.UpdateSupportedDate).IsRequired();
        }
    }
}
