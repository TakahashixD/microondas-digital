using System;
using System.Web.Http;
using backend_microondas.DTOs;
using backend_microondas.Services;

namespace backend_microondas.Controllers
{
    [Authorize]
    [RoutePrefix("api/microondas")]
    public class MicroondasController : ApiController
    {
        private readonly MicroondasService _microondasService;

        public MicroondasController(MicroondasService microondasService)
        {
            _microondasService = microondasService;
        }

        [HttpGet]
        [Route("programas")]
        public IHttpActionResult ObterProgramas()
        {
            try
            {
                var programas = _microondasService.ObterProgramasPreDefinidos();
                return Ok(programas);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter programas: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("programas/{id:int}")]
        public IHttpActionResult ObterPrograma(int id)
        {
            try
            {
                var programa = _microondasService.ObterProgramaPorId(id);
                if (programa == null)
                    return NotFound();

                return Ok(programa);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter programa: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("adicionar")]
        public IHttpActionResult AdicionarProgramaCustomizado([FromBody] ProgramaAquecimentoDTO programa)
        {
            try
            {
                if (programa == null)
                    return BadRequest("Programa inválido.");

                _microondasService.AdicionarProgramaCustomizado(programa);
                return Ok(programa);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao adicionar programa: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("iniciar")]
        public IHttpActionResult IniciarAquecimento([FromBody] MicroondasRequest request)
        {
            try
            {
                var response = _microondasService.IniciarAquecimento(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, new MicroondasResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro interno: {ex.Message}"
                });
            }
        }

        [HttpPost]
        [Route("pausar-cancelar")]
        public IHttpActionResult PausarCancelar()
        {
            try
            {
                var response = _microondasService.PausarCancelar();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, new MicroondasResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro interno: {ex.Message}"
                });
            }
        }

        [HttpGet]
        [Route("status")]
        public IHttpActionResult ObterStatus()
        {
            try
            {
                var status = _microondasService.ObterStatus();
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter status: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("inicio-rapido")]
        public IHttpActionResult InicioRapido()
        {
            try
            {
                var request = new MicroondasRequest(); // Sem tempo nem potência
                var response = _microondasService.IniciarAquecimento(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, new MicroondasResponse
                {
                    Sucesso = false,
                    Mensagem = $"Erro interno: {ex.Message}"
                });
            }
        }
    }
}
