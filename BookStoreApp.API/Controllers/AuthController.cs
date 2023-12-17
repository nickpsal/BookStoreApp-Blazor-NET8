using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models;
using BookStoreApp.API.Models.Dtos.User;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> userManager;
        private readonly IConfiguration configuration;
        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> register(UserDTO userDTO)
        {
            try
            {
                var UserExists = userManager.FindByEmailAsync(userDTO.Email);
                if (UserExists is null)
                {
                    var user = _mapper.Map<ApiUser>(userDTO);
                    user.UserName = userDTO.Email;
                    var result = await userManager.CreateAsync(user, userDTO.Password);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError(error.Code, error.Description);
                            ModelState.AddModelError(error.Code, error.Description);
                        }
                        return BadRequest(ModelState);
                    }
                    await userManager.AddToRoleAsync(user, "User");
                    return Accepted();
                }
                _logger.LogError("User with this email Adress already Exists");
                return Problem("User with this email Adress already Exists");
            }catch (Exception exp)
            {
                _logger.LogError($"Error : {exp}");
                return Problem($"Something went wrong in the {nameof(register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthResponse>> login(LoginUserDTO UserDTO)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(UserDTO.Email);
                var ValidPassword = await userManager.CheckPasswordAsync(user!, UserDTO.Password);
                if (user is null || !ValidPassword)
                {
                    return Unauthorized(UserDTO);
                }
                string tokenString = await GenerateToken(user);
                var resp = new AuthResponse
                {
                    Email = UserDTO.Email,
                    Token = tokenString,
                    userID = user.Id
                };
                return Accepted(resp);
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error : {exp}");
                return Problem($"Something went wrong in the {nameof(register)}", statusCode: 500);
            }
        }

        private async Task<string> GenerateToken(ApiUser user)
        {
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!));
            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var roles = await userManager.GetRolesAsync(user);
            var RoleClaims = roles.Select(q => new Claim(ClaimTypes.Role, q)).ToList();

            var userClaims = await userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email !),
                new Claim(CustomClaimTypes.Uid, user.Id)
            }.Union(RoleClaims).Union(userClaims);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToInt32(configuration["JwtSettings:Duration"])),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
