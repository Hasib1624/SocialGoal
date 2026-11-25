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
    public class GroupCommentUserConfguration : IEntityTypeConfiguration<GroupCommentUser>
    {
        public void Configure(EntityTypeBuilder<GroupCommentUser> builder)
        {
            builder.Property(g => g.GroupCommentId).IsRequired();
            builder.Property(g => g.UserId).IsRequired();
        }
    }
}
