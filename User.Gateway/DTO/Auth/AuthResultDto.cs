using System;

namespace User.Gateway.DTO.Auth
{
    [Serializable]
    public class AuthResultDto
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public DateTime? IssuedAt { get; set; }

        public DateTime? NotBefore { get; set; }

        public DateTime? Expires { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
