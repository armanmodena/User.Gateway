using System.Threading.Tasks;
using Flurl.Http;
using User.Gateway.DTO;
using User.Gateway.DTO.Error;
using User.Gateway.DTO.User;
using User.Gateway.Services.Interfaces;

namespace User.Gateway.Services
{
    public class UserTokenService : IUserTokenService
    {

        private readonly IFLService FLService;

        public UserTokenService(IFLService flService)
        {
            FLService = flService;
        }

        public async Task<(UserTokenDto, Error)> GetByUserId(int user_id)
        {
            var result = await FLService.Request($"user/{user_id}/token").GetAsync().ReceiveJson<ResponseDataDto<UserTokenDto>>();
            return result.Status == 200 ? (result.Data, null) : (null, new Error()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(UserTokenDto, Error)> GetToken(int user_id, string refreshToken)
        {
            var result = await FLService.Request($"user/{user_id}/token/{refreshToken}").GetAsync().ReceiveJson<ResponseDataDto<UserTokenDto>>();
            return result.Status == 200 ? (result.Data, null) : (null, new Error()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(UserTokenDto, Error)> Insert(UserTokenDto userToken)
        {
            var result = await FLService.Request("user/token").PostJsonAsync(userToken).ReceiveJson<ResponseDataDto<UserTokenDto>>();
            return result.Status == 201 ? (result.Data, null) : (null, new Error()
            {
                Status = result.Status,
                Message = result.Message
            });
        }
    }
}
