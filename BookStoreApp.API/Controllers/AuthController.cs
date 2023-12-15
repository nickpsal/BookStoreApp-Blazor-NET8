using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models;
using BookStoreApp.API.Models.Dtos.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> userManager;
        public AuthController(ILogger<AuthController> logger, IMapper mapper, UserManager<ApiUser> userManager)
        {
            _logger = logger;
            _mapper = mapper;
            this.userManager = userManager;
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
        public async Task<IActionResult> login(LoginUserDTO UserDTO)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(UserDTO.Email);
                var ValidPassword = await userManager.CheckPasswordAsync(user!, UserDTO.Password);
                if (user is null || !ValidPassword)
                {
                    return NotFound();
                }
                return Accepted();
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error : {exp}");
                return Problem($"Something went wrong in the {nameof(register)}", statusCode: 500);
            }
        }
    }
}
