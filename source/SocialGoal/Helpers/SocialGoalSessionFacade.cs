using Microsoft.AspNetCore.Http;

namespace SocialGoal.Web.Helpers
{
    public static class SocialGoalSessionFacade
    {
        private const string joinGroupOrGoal = "JoinGroupOrGoal";

        public static IHttpContextAccessor Accessor { get; set; }

        private static ISession Session => Accessor.HttpContext.Session;

        public static string JoinGroupOrGoal
        {
            get { return Session.GetString(joinGroupOrGoal); }
            set
            {
                if (value == null)
                    Session.Remove(joinGroupOrGoal);
                else
                    Session.SetString(joinGroupOrGoal, value);
            }
        }

        public static void Remove(string sessionVariable)
        {
            if (sessionVariable != null)
                Session.Remove(sessionVariable);
        }

        public static void Clear()
        {
            Session.Clear();
        }
    }
}
