using System.Threading.Tasks;
using User.Gateway.DTO;
using User.Gateway.DTO.User;

namespace User.Gateway.Services.Interfaces
{
    public interface IUserService
    {
        Task<(PageResultDto<UserDto>, ErrorDto)> GetAll(string select, string search, string filterAnd, string filterOr, string filterOut, string orderBy, string direction, int page, int pageSize);

        Task<(PageResultDto<UserWithTokenDto>, ErrorDto)> GetAllWithToken(string select, string search, string filterAnd, string filterOr, string filterOut,
            string orderBy, string direction, int page, int pageSize);
        Task<(UserDto, ErrorDto)> Get(int id);

        Task<(UserModenaDto, ErrorDto)> GetModenaUser(string username, string password);

        Task<(UserDto, ErrorDto)> GetByUsername(string username);

        Task<ResponseDataDto> Insert(FormUserDto user);

        Task<ResponseDataDto> Update(int id, FormUserDto user);

        Task<ResponseDataDto> Delete(int id);
    }
}
