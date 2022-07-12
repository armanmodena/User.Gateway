using System;

namespace User.Gateway.DTO.User
{
    public class UserDto
    {
        public int Id { get; set; }

        
        public string Name { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? ImageName { get; set; }
    }
}
