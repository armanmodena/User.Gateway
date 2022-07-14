
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using User.Gateway.DTO.Auth;
using User.Gateway.Services.Interfaces;
using User.Gateway.Utils;

namespace User.Gateway.Controllers{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/auth")]
    public class IdentityController : BaseController
    {
        private readonly IAuthService AuthService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public IdentityController(
           IAuthService authService,
           IHttpContextAccessor _httpContextAccessor)
        {
            AuthService = authService;
            httpContextAccessor = _httpContextAccessor;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginDto authLogin)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var (result, err) = await AuthService.Login(authLogin);
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

        [Authorize]
        [HttpGet("refresh")]
        public async Task<IActionResult> GetRefreshToken([FromQuery] string refresh_token)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string accessToken = httpContextAccessor.HttpContext.Items[ClaimUtil.ACCESS_TOKEN].ToString();
                    if (string.IsNullOrEmpty(accessToken))
                        return HttpResponse(ErrorUtil.TokenNotFound, 401);

                    var (result, err) = await AuthService.GetRefreshToken(accessToken, refresh_token);
                    if (err != null)
                        return HttpResponse(err, 401);

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
