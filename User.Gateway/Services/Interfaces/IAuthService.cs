using System.Threading.Tasks;
using User.Gateway.DTO.Auth;
using User.Gateway.DTO.Error;

namespace User.Gateway.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(AuthResultDto, Error)> Login(AuthLoginDto authLogin);

        Task<(AuthResultDto, Error)> GetRefreshToken(string accessToken, string refreshToken);
    }
}
