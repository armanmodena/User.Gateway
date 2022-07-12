using System.Threading.Tasks;
using User.Gateway.DTO;
using User.Gateway.DTO.Error;
using User.Gateway.DTO.User;

namespace User.Gateway.Services.Interfaces
{
    public interface IUserService
    {
        Task<(PageResult<UserDto>, Error)> GetAll(string select, string search, string filterAnd, string filterOr, string filterOut, string orderBy, string direction, int page, int pageSize);
        Task<(UserDto, Error)> Get(int id);
        Task<(UserDto, Error)> GetByUsername(string username);
    }
}
