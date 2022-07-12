using System.Threading.Tasks;
using User.Gateway.DTO;
using User.Gateway.DTO.Auth;

namespace User.Gateway.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(AuthResultDto, ErrorDto)> Login(AuthLoginDto authLogin);

        Task<(AuthResultDto, ErrorDto)> GetRefreshToken(string accessToken, string refreshToken);
    }
}
