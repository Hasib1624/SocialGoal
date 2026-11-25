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
    public class RegistrationTokenConfiguration : IEntityTypeConfiguration<RegistrationToken>
    {
        public void Configure(EntityTypeBuilder<RegistrationToken> builder)
        {
            builder.Property(r => r.Role).HasMaxLength(50);

        }
    }
}
