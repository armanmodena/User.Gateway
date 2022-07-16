using System.Threading.Tasks;
using Flurl.Http;
using User.Gateway.DTO;
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

        public async Task<(UserTokenDto, ErrorDto)> GetByUserId(int user_id)
        {
            var result = await FLService.Request($"user/{user_id}/token").GetAsync().ReceiveJson<FLResponseDto<UserTokenDto>>();
            return result.Status == 200 ? (result.Data, null) : (null, new ErrorDto()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(UserTokenDto, ErrorDto)> GetToken(int user_id, string refreshToken)
        {
            var result = await FLService.Request($"user/{user_id}/token/{refreshToken}").GetAsync().ReceiveJson<FLResponseDto<UserTokenDto>>();
            return result.Status == 200 ? (result.Data, null) : (null, new ErrorDto()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(UserTokenDto, ErrorDto)> Insert(UserTokenDto userToken)
        {
            var result = await FLService.Request("user/token").PostJsonAsync(userToken).ReceiveJson<FLResponseDto<UserTokenDto>>();
            return result.Status == 201 ? (result.Data, null) : (null, new ErrorDto()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(UserTokenDto, ErrorDto)> Update(int user_id, UserTokenDto userToken)
        {
            var result = await FLService.Request($"user/{user_id}/token").PutJsonAsync(userToken).ReceiveJson<FLResponseDto<UserTokenDto>>();
            return result.Status == 200 ? (result.Data, null) : (null, new ErrorDto()
            {
                Status = result.Status,
                Message = result.Message
            });
        }
    }
}
