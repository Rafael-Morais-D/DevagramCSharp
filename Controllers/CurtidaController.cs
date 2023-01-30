using DevagramCSharp.Dtos;
using DevagramCSharp.Models;
using DevagramCSharp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DevagramCSharp.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CurtidaController : BaseController
    {
        private readonly ILogger<CurtidaController> _logger;
        private readonly ICurtidaRepository _curtidaRepository;

        public CurtidaController(ILogger<CurtidaController> logger, ICurtidaRepository curtidaRepository, IUsuarioRepository usuarioRepository) : base(usuarioRepository)
        {
            _logger = logger;
            _curtidaRepository = curtidaRepository;
        }

        [HttpPut]
        public IActionResult Curtir([FromBody] CurtidaRequisicaoDto curtidadto)
        {
            try
            {
                if (curtidadto != null)
                {
                    Curtida curtida = _curtidaRepository.GetCurtida(curtidadto.IdPublicacao, LerToken().Id);

                    if(curtida != null)
                    {
                        _curtidaRepository.Descurtir(curtida);

                        return Ok($"Usuário {LerToken().Nome} não está mais curtindo a publicação");
                    }
                    else
                    {
                        Curtida curtidaNova = new Curtida()
                        {
                            IdPublicacao = curtidadto.IdPublicacao,
                            IdUsuario = LerToken().Id
                        };

                        _curtidaRepository.Curtir(curtidaNova);

                        return Ok($"Usuário {LerToken().Nome} curtiu a publicação");
                    }
                }

                _logger.LogError("Publicação não existe, então não pode ser curtida");
                throw new Exception("Publicação não existe, então não pode ser curtida");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu um erro ao curtir/descurtir a publicação");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = $"Ocorreu o seguinte erro: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
