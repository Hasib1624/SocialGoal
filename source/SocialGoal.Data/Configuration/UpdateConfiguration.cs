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
    public class UpdateConfiguration : IEntityTypeConfiguration<Update>
    {
        public void Configure(EntityTypeBuilder<Update> builder)
        {
            builder.Property(u => u.Updatemsg);
            builder.Property(u => u.GoalId).IsRequired();
            builder.Property(u => u.UpdateDate).IsRequired();

        }
    }
}
