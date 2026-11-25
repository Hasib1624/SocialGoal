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
    public class UpdateSupportConfiguration : IEntityTypeConfiguration<UpdateSupport>
    {
        public void Configure(EntityTypeBuilder<UpdateSupport> builder)
        {
            builder.Property(u => u.UpdateId).IsRequired();
            builder.Property(u => u.UserId);
            builder.Property(u => u.UpdateSupportedDate).IsRequired();
        }
    }
}
