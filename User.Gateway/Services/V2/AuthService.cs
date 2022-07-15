using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using User.Gateway.DTO;
using User.Gateway.DTO.Auth;
using User.Gateway.DTO.User;
using User.Gateway.Services.Interfaces;
using User.Gateway.Utils;

namespace User.Gateway.Services.V2
{
    public class AuthService : IAuthService
    {
        public readonly ConnectionMultiplexer Redis;
        public readonly IConfiguration Configs;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly JWTDto JWTConfig;
        private readonly IUserService UserService;
        private readonly IUserTokenService UserTokenService;


        public AuthService(
           IUserService userService,
           IUserTokenService userTokenService,
           IOptions<JWTDto> config,
           IHttpContextAccessor httpContextAccessor,
           IConfiguration configs)
        {
            UserService = userService;
            UserTokenService = userTokenService;
            Configs = configs;
            JWTConfig = config.Value;
            HttpContextAccessor = httpContextAccessor;
            Redis = ConnectionMultiplexer.ConnectAsync(Configs["ConnectionStrings:Redis"]).Result;
        }

        public async Task<(AuthResultDto, ErrorDto)> Login(AuthLoginDto authLogin)
        {
            var (user, userError) = await UserService.GetByUsername(authLogin.Username);

            if (userError != null)
                return (null, userError);

            if (authLogin.Username.Contains("@modena.com"))
            {
                var (userModena, userModenaError) = await UserService.GetModenaUser(authLogin.Username, authLogin.Password);
                if (userModenaError != null)
                    return (null, ErrorUtil.PasswordInvalid);
            }
            else
            {
                if (!user.Password.Equals(Hash.EncryptSHA2(authLogin.Password)))
                    return (null, ErrorUtil.PasswordInvalid);
            }

            var userToken = GenerateUserToken(user);
            var (accessToken, accessTokenErr) = await GenerateAccessToken(user, userToken.RefreshToken);

            if (accessTokenErr != null)
                return (null, accessTokenErr);

            int expiryDuration = JWTConfig.ExpiryDuration;

            var result = new AuthResultDto
            {
                Issuer = JWTConfig.Issuer,
                Audience = JWTConfig.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(expiryDuration),
                AccessToken = accessToken,
                RefreshToken = userToken.RefreshToken
            };

            return (result, null);
        }

        private UserTokenDto GenerateUserToken(UserDto user)
        {
            int expiredTime = JWTConfig.RefreshExpiryDuration;

            var expiredOn = DateTime.Now.AddMinutes(expiredTime);
            var refresh = Hash.EncryptSHA2($"id:${user.Id}, expired: ${expiredOn}");

            var userToken = new UserTokenDto();
            userToken.Id = user.Id;
            userToken.UserId = user.Id;
            userToken.RefreshToken = refresh.Replace("=", "");
            userToken.CreatedAt = DateTime.Now;
            userToken.ExpiredAt = expiredOn;

            return userToken;
        }

        private async Task<(string, ErrorDto)> GenerateAccessToken(UserDto user, string refreshToken)
        {
            var claims = new[]
            {
                new Claim(ClaimUtil.USER_ID, user.Id.ToString()),
                new Claim(ClaimUtil.USERNAME, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString())
            };

            var key = Encoding.UTF8.GetBytes(JWTConfig.SigningSecret);
            var signingKey = new SymmetricSecurityKey(key);
            int expiredTime = JWTConfig.ExpiryDuration;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = JWTConfig.Issuer,
                Audience = JWTConfig.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(expiredTime),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.CreateToken(tokenDescriptor);
            var accessToken = jwtTokenHandler.WriteToken(jwtToken);

            int expiryDuration = JWTConfig.ExpiryDuration;
            await Redis.GetDatabase().StringSetAsync($"userapp_{user.Username.ToLower()}_refreshtoken", refreshToken, TimeSpan.FromMinutes(expiryDuration));

            return (accessToken, null);
        }

        public async Task<(AuthResultDto, ErrorDto)> GetRefreshToken(string accessToken, string refreshToken)
        {
            var userId = HttpContextAccessor.HttpContext.Items[ClaimUtil.USER_ID].ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                var id = int.Parse(userId);

                var (user, userError) = await UserService.Get(id);
                if (userError != null)
                    return (null, userError);

                var userToken = GenerateUserToken(user);

                var currentAccessToken = Redis.GetDatabase().StringGet($"userapp_{user.Username.ToLower()}_refreshtoken");
                if (string.IsNullOrEmpty(currentAccessToken))
                    return (null, ErrorUtil.RefreshTokenNotFound);

                if (currentAccessToken != refreshToken)
                    return (null, ErrorUtil.TokenInvalid);

                var (newAccessToken, newAccessTokenErr) = await GenerateAccessToken(user, userToken.RefreshToken);
                if (newAccessTokenErr != null)
                    return (null, newAccessTokenErr);

                int expiryDuration = JWTConfig.ExpiryDuration;
                var result = new AuthResultDto
                {
                    Issuer = JWTConfig.Issuer,
                    Audience = JWTConfig.Audience,
                    IssuedAt = DateTime.Now,
                    NotBefore = DateTime.Now,
                    Expires = DateTime.Now.AddMinutes(expiryDuration),
                    AccessToken = newAccessToken,
                    RefreshToken = userToken.RefreshToken
                };
                return (result, null);
            }

            return (null, ErrorUtil.InvalidOperation);
        }
    }
}
