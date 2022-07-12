using System.Threading.Tasks;
using Flurl.Http;
using User.Gateway.DTO;
using User.Gateway.DTO.Error;
using User.Gateway.DTO.User;
using User.Gateway.Services.Interfaces;

namespace User.Gateway.Services
{
    public class UserService : IUserService
    {
        private readonly IFLService FLService;

        public UserService(IFLService flService)
        {
            FLService = flService;
        }

        public async Task<(PageResult<UserDto>, Error)> GetAll(string select, string search, string filterAnd, string filterOr, string filterOut, 
            string orderBy, string direction, int page, int pageSize)
        {
            var result = await FLService.Request("user").SetQueryParams(new
            {
                select = select != null ? select : "",
                search = search != null ? search : "",
                filterAnd = filterAnd != null ? filterAnd : "",
                filterOr = filterOr != null ? filterOr : "",
                filterOut = filterOut != null ? filterOut : "",
                orderBy = orderBy != null ? orderBy : "",
                direction = direction != null ? direction : "",
                page = page != null ? page : 1,
                pageSize = pageSize != null ? pageSize : 10,
            }).GetAsync().ReceiveJson<ResponseDataDto<PageResult<UserDto>>>();
            return result.Status == 200 ? (result.Data, null) : (null, new Error()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(UserDto, Error)> Get(int id)
        {
            var result = await FLService.Request($"user/{id}").GetAsync().ReceiveJson<ResponseDataDto<UserDto>>();
            return result.Status == 200 ? (result.Data, null) : (null, new Error()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(UserDto, Error)> GetByUsername(string username)
        {
            var result = await FLService.Request($"user/{username}/username").GetAsync().ReceiveJson<ResponseDataDto<UserDto>>();
            return result.Status == 200 ? (result.Data, null) : (null, new Error() { 
                Status = result.Status, 
                Message = result.Message 
            });
        }
    }
}
