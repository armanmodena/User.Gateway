using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Gateway.DTO.User;
using User.Gateway.Services.Interfaces;

namespace User.Gateway.Controllers.V2
{
    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{v:apiVersion}/user")]
    public class UserController : BaseController
    {
        public readonly IUserService UserService;

        private readonly IFileUploadService FileUploadService;

        public UserController(
            IUserService userService,
            IFileUploadService fileUploadService)
        {
            UserService = userService;
            FileUploadService = fileUploadService;
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
        public async Task<IActionResult> Post([FromForm] FormUserDto user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var fileName = FileUploadService.genImageName(Request.Form.Files, "image");

                    user.ImageName = fileName != null ? fileName : null;

                    var result = await UserService.Insert(user);

                    if(result.Status == 201 && fileName != null)
                    {
                        var dbPath = FileUploadService.saveImage(Request.Form.Files, "image", fileName);
                    }

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
        public async Task<IActionResult> Put(int id, [FromForm] FormUserDto user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var (oldUser ,oldUserError) = await UserService.Get(id);
                    
                    var fileName = FileUploadService.genImageName(Request.Form.Files, "image");

                    user.ImageName = fileName != null ? fileName : null;

                    var result = await UserService.Update(id, user);

                    if (result.Status == 200 && fileName != null)
                    {
                        var dbPath = FileUploadService.saveImage(Request.Form.Files, "image", fileName);

                        if (!String.IsNullOrEmpty(dbPath) && oldUser.ImageName != null)
                            FileUploadService.deleteImage(oldUser.ImageName);
                    }

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
                    var ImageName = result.Data.GetType().GetProperty("ImageName").GetValue(result.Data, null);
                    if (ImageName != null)
                    {
                        FileUploadService.deleteImage(ImageName.ToString());
                    }

                    return HttpResponse(result);
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
