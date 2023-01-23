using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SI.Identity.Models;
using SI.Identity.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SI.Identity.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtGenerator _jwtGenerator;

        public AccountController(
            UserManager<User> userManager,
            IJwtGenerator jwtGenerator)
        {
            this._userManager = userManager;
            this._jwtGenerator = jwtGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new { Message = "User already exists!" });

            var user = new User()
            {
                UserName = model.Email,
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Patronymic = model.Patronymic
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var jwtResponse = await _jwtGenerator.Generate(user);
                return Ok(jwtResponse);
            }
            return Unauthorized();
        }
    }
}
