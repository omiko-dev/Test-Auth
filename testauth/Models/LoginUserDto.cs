using Microsoft.AspNetCore.Identity;

namespace testauth.Models
{
    public class LoginUserDto
    {

        public required string Name { get; set; }

        public required string Password { get; set; }

    }
}
