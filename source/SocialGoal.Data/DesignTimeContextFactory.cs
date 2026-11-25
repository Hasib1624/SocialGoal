using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SocialGoal.Data.Models;

namespace SocialGoal.Data
{
    // Used only by "dotnet ef"; adding a migration needs no running server.
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<SocialGoalEntities>
    {
        public SocialGoalEntities CreateDbContext(string[] args)
        {
            var connection = Environment.GetEnvironmentVariable("ConnectionStrings__SocialGoalEntities") ?? "Server=localhost;Database=SocialGoal";
            return new SocialGoalEntities(new DbContextOptionsBuilder<SocialGoalEntities>().UseSqlServer(connection).Options);
        }
    }
}
