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
   public  class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
       public void Configure(EntityTypeBuilder<UserProfile> builder)
       {
           builder.Property(u => u.FirstName).HasMaxLength(100);
           builder.Property(u => u.LastName);
           builder.Property(u => u.DateEdited).IsRequired();
           builder.Property(u => u.Email);
           builder.Property(u => u.Address);
           builder.Property(u => u.City).HasMaxLength(100);
           builder.Property(u => u.Country).HasMaxLength(50);
           builder.Property(u => u.State).HasMaxLength(50);
           builder.Property(u => u.UserId).HasMaxLength(50);
       }
    }
}
