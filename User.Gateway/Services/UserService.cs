using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using User.Gateway.DTO;
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

        public async Task<(PageResultDto<UserDto>, ErrorDto)> GetAll(string select, string search, string filterAnd, string filterOr, string filterOut,
            string orderBy, string direction, int page, int pageSize)
        {
            var result = await FLService.Request("user").SetQueryParams(new
            {
                select = select,
                search = search,
                filterAnd = filterAnd,
                filterOr = filterOr,
                filterOut = filterOut,
                orderBy = orderBy,
                direction = direction,
                page = page,
                pageSize = pageSize,
            }).GetAsync().ReceiveJson<FLResponseDto<PageResultDto<UserDto>>>();
            return result.Status == 200 ? (result.Data, null) : (null, new ErrorDto()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(PageResultDto<UserWithTokenDto>, ErrorDto)> GetAllWithToken(string select, string search, string filterAnd, string filterOr, string filterOut,
            string orderBy, string direction, int page, int pageSize)
        {
            var result = await FLService.Request("user/with-token").SetQueryParams(new
            {
                select = select,
                search = search,
                filterAnd = filterAnd,
                filterOr = filterOr,
                filterOut = filterOut,
                orderBy = orderBy,
                direction = direction,
                page = page,
                pageSize = pageSize,
            }).GetAsync().ReceiveJson<FLResponseDto<PageResultDto<UserWithTokenDto>>>();
            return result.Status == 200 ? (result.Data, null) : (null, new ErrorDto()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(UserDto, ErrorDto)> Get(int id)
        {
            var result = await FLService.Request($"user/{id}").GetAsync().ReceiveJson<FLResponseDto<UserDto>>();
            return result.Status == 200 ? (result.Data, null) : (null, new ErrorDto()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(UserDto, ErrorDto)> GetByUsername(string username)
        {
            var result = await FLService.Request($"user/{username}/username").GetAsync().ReceiveJson<FLResponseDto<UserDto>>();
            return result.Status == 200 ? (result.Data, null) : (null, new ErrorDto()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<(UserModenaDto, ErrorDto)> GetModenaUser(string username, string password)
        {
            var result = await FLService.IdentityRequest($"user/validate").PostJsonAsync(new
            {
                username = username,
                password = password
            }).ReceiveJson<FLResponseModenaIdentity<UserModenaDto>>();
            return result.Status == 200 ? (result.User, null) : (null, new ErrorDto()
            {
                Status = result.Status,
                Message = result.Message
            });
        }

        public async Task<ResponseDataDto> Insert(FormUserDto user)
        {
            var result = await FLService.Request("user").PostJsonAsync(user).ReceiveJson<FLResponseDto<UserDto>>();
            return new ResponseDataDto()
            {
                Status = result.Status,
                Message = result.Message,
                Data = result.Data,
                Errors = result.Errors != null ? FLService.FormErrors(result.Errors) : null
            };
        }

        public async Task<ResponseDataDto> InsertImport(List<UserDto> users)
        {
            var result = await FLService.Request("user/import").PostJsonAsync(users).ReceiveJson<FLResponseDto<UserDto>>();
            return new ResponseDataDto()
            {
                Status = result.Status,
                Message = result.Message,
                Data = result.Data,
                Errors = result.Errors != null ? FLService.FormErrors(result.Errors) : null
            };
        }

        public async Task<ResponseDataDto> Update(int id, FormUserDto user)
        {
            var result = await FLService.Request($"user/{id}").PutJsonAsync(user).ReceiveJson<FLResponseDto<UserDto>>();
            return new ResponseDataDto()
            {
                Status = result.Status,
                Message = result.Message,
                Data = result.Data,
                Errors = result.Errors != null ? FLService.FormErrors(result.Errors) : null
            };
        }

        public async Task<ResponseDataDto> Delete(int id)
        {
            var result = await FLService.Request($"user/{id}").DeleteAsync().ReceiveJson<FLResponseDto<UserDto>>();
            return new ResponseDataDto()
            {
                Status = result.Status,
                Message = result.Message,
                Data = result.Data,
                Errors = result.Errors != null ? FLService.FormErrors(result.Errors) : null
            };
        }
    }
}