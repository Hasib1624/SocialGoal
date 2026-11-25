using SocialGoal.Data.Models;

namespace SocialGoal.Data.Infrastructure
{
    // The DI container owns the context's lifetime.
    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private readonly SocialGoalEntities dataContext;

        public DatabaseFactory(SocialGoalEntities dataContext)
        {
            this.dataContext = dataContext;
        }

        public SocialGoalEntities Get()
        {
            return dataContext;
        }
    }
}
