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
    public class CommentUserConfiguration : IEntityTypeConfiguration<CommentUser>
    {
        public void Configure(EntityTypeBuilder<CommentUser> builder)
        {
            builder.Property(c => c.CommentId).IsRequired();
            builder.Property(c => c.UserId).IsRequired();
        }
    }
}
