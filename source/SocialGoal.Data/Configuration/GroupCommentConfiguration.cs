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
    public class GroupCommentConfiguration : IEntityTypeConfiguration<GroupComment>
    {
        public void Configure(EntityTypeBuilder<GroupComment> builder)
        {
            builder.Property(g => g.GroupUpdateId).IsRequired();
            builder.Property(g => g.CommentText);
            builder.Property(g => g.CommentDate).IsRequired();
        }
    }
}
