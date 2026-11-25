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
    public class GroupInvitationConfiguration : IEntityTypeConfiguration<GroupInvitation>
    {
        public void Configure(EntityTypeBuilder<GroupInvitation> builder)
        {
            builder.Property(g => g.FromUserId);
            builder.Property(g => g.ToUserId);
            builder.Property(g => g.GroupId).IsRequired();
            builder.Property(g => g.SentDate).IsRequired();
            builder.Property(g => g.Accepted).IsRequired();
        }
    }
}
