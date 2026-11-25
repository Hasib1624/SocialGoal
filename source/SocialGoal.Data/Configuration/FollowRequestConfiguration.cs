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
    public  class FollowRequestConfiguration : IEntityTypeConfiguration<FollowRequest>
    {
        public void Configure(EntityTypeBuilder<FollowRequest> builder)
        {
            builder.Property(f => f.FromUserId).IsRequired();
            builder.Property(f => f.ToUserId).IsRequired();
            builder.Property(f => f.Accepted).IsRequired();
        }
    }
}
