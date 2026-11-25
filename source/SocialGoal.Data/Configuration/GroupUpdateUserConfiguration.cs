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
    public class GroupUpdateUserConfiguration : IEntityTypeConfiguration<GroupUpdateUser>
    {
        public void Configure(EntityTypeBuilder<GroupUpdateUser> builder)
        {
            builder.Property(g => g.GroupUpdateUserId).IsRequired();
            builder.Property(g => g.UserId).IsRequired();
        }
    }
}
