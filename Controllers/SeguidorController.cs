using DevagramCSharp.Dtos;
using DevagramCSharp.Models;
using DevagramCSharp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DevagramCSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeguidorController : BaseController
    {
        private readonly ILogger<SeguidorController> _logger;
        private readonly ISeguidorRepository _seguidorRepository;

        public SeguidorController (ILogger<SeguidorController> logger, ISeguidorRepository seguidorRepository, IUsuarioRepository usuarioRepository) : base(usuarioRepository)
        {
            _logger = logger;
            _seguidorRepository = seguidorRepository;
        }

        [HttpPut]
        public IActionResult Seguir (int idSeguido)
        {
            try
            {
                Usuario usuarioSeguido = _usuarioRepository.GetUsuarioPorId(idSeguido);
                Usuario usuarioSeguidor = LerToken();

                if (usuarioSeguido != null)
                {
                    Seguidor seguidor = _seguidorRepository.GetSeguidor(usuarioSeguidor.Id, usuarioSeguido.Id);

                    if (seguidor != null)
                    {
                        _seguidorRepository.Desseguir(seguidor);

                        return Ok($"Usuário {usuarioSeguidor.Nome} não está mais seguindo {usuarioSeguido.Nome}");
                    }
                    else
                    {
                        Seguidor seguidorNovo = new Seguidor()
                        {
                            IdUsuarioSeguido = usuarioSeguido.Id,
                            IdUsuarioSeguidor = usuarioSeguidor.Id
                        };
                        _seguidorRepository.Seguir(seguidorNovo);

                        return Ok($"Usuário {usuarioSeguidor.Nome} agora está seguindo {usuarioSeguido.Nome}");
                    }
                }

                throw new Exception("Usuário não existe, então não pode ser seguido");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu um erro ao seguir/desseguir o usuário");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = $"Ocorreu o seguinte erro: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
