using AuthorizationServer.Data;
using AuthorizationServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthorizationServer.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {

            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Sub, user.UserName ?? user.Email),
                    new(ClaimTypes.Name, user.UserName ?? user.Email),
                    new(ClaimTypes.MobilePhone,user.PhoneNumber ?? ""),
                    new(ClaimTypes.Email,user.Email),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                //--Https://github.com/SardarMudassarAliKhan/JWTTokenAuthInAspNet6
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSettings:IssuerSigningKey"]));
                var token = new JwtSecurityToken(
                    issuer: _configuration["JWTSettings:ValidIssuer"],  //missing in code sample
                    audience: _configuration["JWTSettings:ValidAudience"], //missing in code sample  (not used due to ValidateAudience: false)
                    claims: authClaims,
                    expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["JWTSettings:DurationInHours"])), //added as Configuration setting
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    api_key = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    //user,
                    Role = userRoles,
                    status = "User Login Success"
                });
            }
            return Unauthorized();
        }
    }
}
