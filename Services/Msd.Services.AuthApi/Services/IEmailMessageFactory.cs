using Msd.Integration.MessageBus.Models.Dtos;

namespace Msd.Services.AuthApi.Services
{
    public interface IEmailMessageFactory
    {
        SendEmailRequestDto CreateRegisterUserEmail(string email);

        SendEmailRequestDto CreateDeleteUserEmail(string email);

        SendEmailRequestDto CreateAddRoleToUserEmail(string email, string role);

        SendEmailRequestDto CreateRemoveRoleToUserEmail(string email, string role);

    }
}
