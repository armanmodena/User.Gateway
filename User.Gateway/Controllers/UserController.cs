 using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Gateway.Services.Interfaces;

namespace User.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    [Route("user")]
    public class UserController : BaseController
    {
        public readonly IUserService UserService;

        public UserController(
            IUserService userService)
        {
            UserService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string select = "*", [FromQuery] string search = null, [FromQuery] string filterAnd = null, [FromQuery] string filterOr = null,
            [FromQuery] string filterOut = null, [FromQuery] string order = "id", [FromQuery] string direction = "asc", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var (result, err) = await UserService.GetAll(select, search, filterAnd, filterOr, filterOut, order, direction, page, pageSize);
                    if (err != null)
                        return HttpResponse(err);

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return ErrorResponse(ex);
                }
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var (result, err) = await UserService.Get(id);
                    if (err != null)
                        return HttpResponse(err);

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return ErrorResponse(ex);
                }
            }
            return BadRequest();
        }
    }
}
