using UI.Models;

namespace UI.Services
{
    public interface IUserService
    {
        Task<Response> RegisterUser(RegistrationModel registrationModel);
        Task<Response> LoginUser(LoginModel loginModel);

    }
}
