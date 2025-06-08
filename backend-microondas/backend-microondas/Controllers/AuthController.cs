using System.Threading.Tasks;
using System.Web.Http;
using backend_microondas.DTOs;
using backend_microondas.Services;

namespace backend_microondas.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Auth(request);

            if (!result.Sucesso)
            {
                return Unauthorized(); 
            }

            return Ok(result);
        }
    }
}
