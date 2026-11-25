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
    public class GroupRequestConfiguration : IEntityTypeConfiguration<GroupRequest>
    {
        public void Configure(EntityTypeBuilder<GroupRequest> builder)
        {
            builder.Property(g => g.GroupId).IsRequired();
            builder.Property(g => g.Accepted).IsRequired();
        }
    }
}
