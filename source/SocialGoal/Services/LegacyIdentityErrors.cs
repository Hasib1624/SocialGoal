using Microsoft.AspNetCore.Identity;

namespace SocialGoal.Web.Services
{
    /// <summary>Message texts of ASP.NET Identity 1.0, which users (and the tests) saw before.</summary>
    public class LegacyIdentityErrors : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName) =>
            new IdentityError { Code = nameof(DuplicateUserName), Description = $"Name {userName} is already taken." };

        public override IdentityError PasswordTooShort(int length) =>
            new IdentityError { Code = nameof(PasswordTooShort), Description = $"Passwords must be at least {length} characters." };

        public override IdentityError InvalidUserName(string userName) =>
            new IdentityError { Code = nameof(InvalidUserName), Description = $"User name {userName} is invalid, can only contain letters or digits." };

        public override IdentityError PasswordMismatch() =>
            new IdentityError { Code = nameof(PasswordMismatch), Description = "Incorrect password." };
    }
}
