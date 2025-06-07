using microondas_backend.DTOs;
using microondas_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace microondas_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _authService;

        public AuthController(AuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Auth(request);

            if (!result.Sucesso)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}
