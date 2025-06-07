using microondas_backend.Models;
using microondas_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace microondas_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MicroondasController : ControllerBase
    {
        private readonly MicroondasService _microondasService;

        public MicroondasController(MicroondasService microondasService)
        {
            _microondasService = microondasService;
        }

        [HttpGet("programas")]
        public ActionResult<List<ProgramaAquecimento>> ObterProgramas()
        {
            try
            {
                var programas = _microondasService.ObterProgramasPreDefinidos();
                return Ok(programas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpGet("programas/{id}")]
        public ActionResult<ProgramaAquecimento> ObterPrograma(int id)
        {
            try
            {
                var programa = _microondasService.ObterProgramaPorId(id);
                if (programa == null)
                    return NotFound("Programa não encontrado.");

                return Ok(programa);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpPost("adicionar")]
        public ActionResult<ProgramaAquecimento> AdicionarProgramaCustomizado(ProgramaAquecimento programa)
        {
            try
            {
                if (programa == null)
                    return Forbid("Não é possível adicionar null.");

               _microondasService.AdicionarProgramaCustomizado(programa);

                return Ok(programa);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpPost("iniciar")]
        public ActionResult<MicroondasResponse> IniciarAquecimento([FromBody] MicroondasRequest request)
        {
            try
            {
                var response = _microondasService.IniciarAquecimento(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new MicroondasResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro interno: {ex.Message}"
                });
            }
        }

        [HttpPost("pausar-cancelar")]
        public ActionResult<MicroondasResponse> PausarCancelar()
        {
            try
            {
                var response = _microondasService.PausarCancelar();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new MicroondasResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro interno: {ex.Message}"
                });
            }
        }

        [HttpGet("status")]
        public ActionResult<StatusResponse> ObterStatus()
        {
            try
            {
                var status = _microondasService.ObterStatus();
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = $"Erro em obter status atual do microondas: {ex.Message}" });
            }
        }

        [HttpPost("inicio-rapido")]
        public ActionResult<MicroondasResponse> InicioRapido()
        {
            try
            {
                var request = new MicroondasRequest(); // Sem tempo nem potência
                var response = _microondasService.IniciarAquecimento(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new MicroondasResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro interno: {ex.Message}"
                });
            }
        }
    }
}
