using System;

namespace User.Gateway.DTO.User
{
    public class UserTokenDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string RefreshToken { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiredAt { get; set; }
    }
}
