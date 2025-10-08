using Microsoft.AspNetCore.Mvc;
using NuLogicEHR.Services;
using NuLogicEHR.ViewModels;

namespace NuLogicEHR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequestViewModel request)
        {
            // Add your authentication logic here
            if (request.Email == "admin@nulogic.com" && request.Password == "password")
            {
                var token = _jwtService.GenerateToken("1", request.Email, "tenant1");
                return Ok(new { Token = token, Message = "Login successful" });
            }

            return Unauthorized(new { Message = "Invalid credentials" });
        }
    }
}