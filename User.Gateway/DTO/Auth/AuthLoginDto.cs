using System.ComponentModel.DataAnnotations;

namespace User.Gateway.DTO.Auth
{
    public class AuthLoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
