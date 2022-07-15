using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Gateway.DTO.User;
using User.Gateway.Services.Interfaces;
using User.Gateway.Utils;

namespace User.Gateway.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/user")]
    public class UserController : BaseController
    {
        public readonly IUserService UserService;

        public readonly IExportFileService ExportFileService;

        public UserController(
            IUserService userService, 
            IExportFileService exportFileService)
        {
            UserService = userService;
            ExportFileService = exportFileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string select, [FromQuery] string search, [FromQuery] string filterAnd, [FromQuery] string filterOr,
            [FromQuery] string filterOut, [FromQuery] string order = "id", [FromQuery] string direction = "asc", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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

        [HttpGet("with-token")]
        public async Task<IActionResult> GetAllWithToken([FromQuery] string select, [FromQuery] string search, [FromQuery] string filterAnd, [FromQuery] string filterOr,
            [FromQuery] string filterOut, [FromQuery] string order = "id", [FromQuery] string direction = "asc", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var (result, err) = await UserService.GetAllWithToken(select, search, filterAnd, filterOr, filterOut, order, direction, page, pageSize);
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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FormUserDto user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await UserService.Insert(user);
                    return HttpResponse(result);
                }
                catch (Exception ex)
                {
                    return ErrorResponse(ex);
                }
            }
            return BadRequest();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] FormUserDto user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await UserService.Update(id, user);
                    return HttpResponse(result);
                }
                catch (Exception ex)
                {
                    return ErrorResponse(ex);
                }
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await UserService.Delete(id);
                    return HttpResponse(result);
                }
                catch (Exception ex)
                {
                    return ErrorResponse(ex);
                }
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportToCSV()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var (result, err) = await UserService.GetAll(null, null, null, null, null, "id", "asc", 1, 1000000);
                    if (err != null)
                        return HttpResponse(err);

                    string[] fields = { "Id", "Name", "Username" };
                    return File(ExportFileService.ExportCSV<UserDto>(result.Data, fields), ExportUtil.CSVType, "user.csv");
                }
                catch (Exception ex)
                {
                    return ErrorResponse(ex);
                }
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("with-token/export-excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var (result, err) = await UserService.GetAllWithToken(null, null, null, null, null, "id", "asc", 1, 1000000);
                    if (err != null)
                        return HttpResponse(err);

                    string[] fields = { "Id", "Name", "Username", "RefreshToken" };
                    return File(ExportFileService.ExportExcel<UserWithTokenDto>(result.Data, fields), ExportUtil.ExcelType, "users.xlsx");
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
