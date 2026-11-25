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
   public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
       public void Configure(EntityTypeBuilder<Comment> builder)
       {
           builder.Property(c => c.CommentText).HasMaxLength(250);
       }
    }
}
