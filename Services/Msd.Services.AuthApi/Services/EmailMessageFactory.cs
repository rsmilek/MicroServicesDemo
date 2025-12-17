using Msd.Integration.MessageBus.Models.Dtos;

namespace Msd.Services.AuthApi.Services
{
    public class EmailMessageFactory : IEmailMessageFactory
    {
        public SendEmailRequestDto CreateRegisterUserEmail(string email)
        {
            return new SendEmailRequestDto
            {
                To = email,
                Subject = "Welcome to MSD Application",
                Body = $"Hello {email},\n\nThank you for registering at MSD Application.\n\nBest regards,\nMSD Team"
            };
        }

        public SendEmailRequestDto CreateDeleteUserEmail(string email)
        {
            return new SendEmailRequestDto
            {
                To = email,
                Subject = "Account Deletion Notification",
                Body = $"Hello {email},\n\nYour account has been successfully deleted from MSD Application.\n\nBest regards,\nMSD Team"
            };
        }

        public SendEmailRequestDto CreateAddRoleToUserEmail(string email, string role)
        {
            return new SendEmailRequestDto
            {
                To = email,
                Subject = "Role Assignment Notification",
                Body = $"Hello {email},\n\nYou have been assigned the role of '{role}' in the MSD Application.\n\nBest regards,\nMSD Team"
            };
        }

        public SendEmailRequestDto CreateRemoveRoleToUserEmail(string email, string role)
        {
            return new SendEmailRequestDto
            {
                To = email,
                Subject = "Role Removal Notification",
                Body = $"Hello {email},\n\nThe role of '{role}' has been removed from your account in the MSD Application.\n\nBest regards,\nMSD Team"
            };
        }

    }
}
