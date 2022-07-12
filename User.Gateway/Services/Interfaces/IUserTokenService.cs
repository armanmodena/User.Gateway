using System.Threading.Tasks;
using User.Gateway.DTO;
using User.Gateway.DTO.User;

namespace User.Gateway.Services.Interfaces
{
    public interface IUserTokenService
    {
        Task<(UserTokenDto, ErrorDto)> GetByUserId(int user_id);

        Task<(UserTokenDto, ErrorDto)> GetToken(int user_id, string refreshToken);

        Task<(UserTokenDto, ErrorDto)> Insert(UserTokenDto userToken);

    }
}
