using Mango.Services.EmailAPI.Models;

namespace Mango.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
        Task EmailUserRegistration(RegistrationRequestDto registrationRequestDto);
    }
}
