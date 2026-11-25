using System;

namespace SocialGoal.Web.Mailers
{
    public interface IUserMailer
    {
        OutgoingMail Welcome();
        OutgoingMail Invite(string email, Guid groupIdToken);
        OutgoingMail Support(string email, Guid goalIdToken);
        OutgoingMail ResetPassword(string email, Guid passwordResetToken);
        OutgoingMail InviteNewUser(string email, Guid registrationToken);
    }
}
