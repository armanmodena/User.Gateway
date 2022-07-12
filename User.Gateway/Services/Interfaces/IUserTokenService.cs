using System.Threading.Tasks;
using User.Gateway.DTO.Error;
using User.Gateway.DTO.User;

namespace User.Gateway.Services.Interfaces
{
    public interface IUserTokenService
    {
        Task<(UserTokenDto, Error)> GetByUserId(int user_id);

        Task<(UserTokenDto, Error)> GetToken(int user_id, string refreshToken);

        Task<(UserTokenDto, Error)> Insert(UserTokenDto userToken);

    }
}
