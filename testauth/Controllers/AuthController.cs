using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using testauth.Models;
using testauth.Services;

namespace testauth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IUserInfoRepository _userInfo;

        public AuthController(IConfiguration configuration, IUserInfoRepository userInfo)
        {
            _configuration = configuration;
            _userInfo = userInfo;
        }

        [HttpPost("Register")]
        public ActionResult<User> register(UserDto _user)
        {


            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(_user.Password);


            user.PasswordHash = PasswordHash;
            user.Name = _user.Name;
            user.Email = _user.Email;
            user.Role = "user";

            return Ok(user);

        }

        [HttpPost("Login")]
        public ActionResult<User> Login(LoginUserDto _user)
        {

            if (user.Name != _user.Name)
                return BadRequest("Name is Incorrect!");

            if (!BCrypt.Net.BCrypt.Verify(_user.Password, user.PasswordHash))
                return BadRequest("Password is Incorrect!");

            var _token = token(user);

            var refreshToken = CreateRefreshToken();

            setRefreshToken(refreshToken);

            return Ok(_token);


        }



        [HttpPost("Refresh-Token")]
        public async Task<ActionResult<string>> RefreshToken()
        {

            var refreshToken = Request.Cookies["Refresh_Token"];
            await Console.Out.WriteLineAsync(refreshToken);
            await Console.Out.WriteLineAsync(user.RefreshToken);

            if (!user.RefreshToken.Equals(refreshToken))
            {
                return BadRequest("Invalid Refresh Token");
            }

            if(user.TokenExpires < DateTime.Now)
            {
                return BadRequest("Token Expires!");
            }

            var newToken = token(user);

            var newRefreshToken = CreateRefreshToken();

            setRefreshToken(newRefreshToken);

            return Ok(newToken);
        }
        private void setRefreshToken(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.TokenExpires
            };

            Response.Cookies.Append("Refresh_Token", refreshToken.token, cookieOptions);

            user.RefreshToken = refreshToken.token;
            user.TokenCreate = refreshToken.TokenCreate;
            user.TokenExpires = refreshToken.TokenExpires; 

        }

        private RefreshToken CreateRefreshToken()
        {

            var refreshToken = new RefreshToken
            {
                token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                TokenExpires = DateTime.Now.AddDays(7)
            };

            return refreshToken;
        }



        [HttpGet("User"), Authorize]
        public ActionResult<string> GetMe()
        {

            var userInfo = _userInfo.GetUserInfo()!.Value;

            return Ok(userInfo);

        }

        


        private string token(User user)
        {

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
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
