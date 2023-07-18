using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using testauth.Models;

namespace testauth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        public static User user = new User();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("Register")]
        public ActionResult<User> register(UserDto _user)
        {


            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(_user.Password);


            user.PasswordHash = PasswordHash;
            user.Name = _user.Name;
            user.Role = _user.Role;

            return Ok(user);

        }

        [HttpPost("Login")]
        public ActionResult<User> Login(UserDto _user)
        {

            if (user.Name != _user.Name)
                return BadRequest("Name is Incorrect!");

            if (!BCrypt.Net.BCrypt.Verify(_user.Password, user.PasswordHash))
                return BadRequest("Password is Incorrect!");

            var _token = token(user);

            return Ok(_token);


        }



        private string token(User user)
        {

            List<Claim> claims = new List<Claim>()
            {
                new Claim("name", user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JWT:token").Value!));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha384Signature);

            var token = new JwtSecurityToken
                (
                   claims: claims,
                   expires: DateTime.Now.AddDays(1),
                   signingCredentials: cred
                );

            var Jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Jwt;

        }



    }
}
