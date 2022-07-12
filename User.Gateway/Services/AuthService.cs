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
using User.Gateway.DTO.Error;
using User.Gateway.DTO.User;
using User.Gateway.Services.Interfaces;
using User.Gateway.Utils;

namespace User.Gateway.Services
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

        public async Task<(AuthResultDto, Error)> Login(AuthLoginDto authLogin)
        {
            var (user, userError) = await UserService.GetByUsername(authLogin.Username);

            if (userError != null) 
                return (null, userError);  

            if (!user.Password.Equals(Hash.EncryptSHA2(authLogin.Password))) 
                return (null, ErrorUtil.PasswordInvalid);

            var userToken = await GenerateUserToken(user);
            var (accessToken, accessTokenErr) = await GenerateAccessToken(user, userToken);

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

        private async Task<UserTokenDto> GenerateUserToken(UserDto user)
        {
            int expiredTime = JWTConfig.RefreshExpiryDuration;
            var (userToken, userError) = await UserTokenService.GetByUserId(user.Id);

            if (userError != null || userToken.ExpiredAt < DateTime.Now)
            {
                var expiredOn = DateTime.Now.AddMinutes(expiredTime);
                var refresh = Hash.EncryptSHA2($"id:${user.Id}, expired: ${expiredOn}");

                userToken = new UserTokenDto();
                userToken.UserId = user.Id;
                userToken.RefreshToken = refresh.Replace("=", "");
                userToken.CreatedAt = DateTime.Now;
                userToken.ExpiredAt = expiredOn;

                var (savedToken, tokenError) = await UserTokenService.Insert(userToken);

                userToken.Id = savedToken.Id;

            }
            return userToken;
        }

        private async Task<(string, Error)> GenerateAccessToken(UserDto user, UserTokenDto userToken)
        {
            var claims = new[]
            {
                new Claim(ClaimUtil.TOKEN_ID, userToken.Id.ToString()),
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
            var currentAccessToken = Redis.GetDatabase().StringGet($"userapp_{user.Username.ToLower()}_accesstoken");

            if (!string.IsNullOrEmpty(currentAccessToken))
                return (null, ErrorUtil.CannotLoginRightNow);
            else
                await Redis.GetDatabase().StringSetAsync($"userapp_{user.Username.ToLower()}_accesstoken", accessToken, TimeSpan.FromMinutes(expiryDuration));

            return (accessToken, null);
        }

        public async Task<(AuthResultDto, Error)> GetRefreshToken(string accessToken, string refreshToken)
        {
            var userId = HttpContextAccessor.HttpContext.Items[ClaimUtil.USER_ID].ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                var id = Int32.Parse(userId);

                var (user, userError) = await UserService.Get(id);
                if(userError != null)
                    return (null, userError);

                var (userToken, tokenError) = await UserTokenService.GetToken(id, refreshToken);
                if(tokenError != null)
                    return (null, tokenError);

                var (newAccessToken, newAccessTokenErr) = await GenerateAccessToken(user, userToken);
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
