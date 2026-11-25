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
    public class SupportInvitationConfiguration : IEntityTypeConfiguration<SupportInvitation>
    {
        public void Configure(EntityTypeBuilder<SupportInvitation> builder)
        {
            builder.Property(s => s.GoalId).IsRequired();
            builder.Property(s => s.SentDate).IsRequired();
            builder.Property(s => s.Accepted).IsRequired();
            builder.Property(s => s.FromUserId);
            builder.Property(s => s.ToUserId);
        }
    }
}
