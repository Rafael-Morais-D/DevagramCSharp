using DevagramCSharp.Dtos;
using DevagramCSharp.Models;
using DevagramCSharp.Repository;
using DevagramCSharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevagramCSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComentarioController : BaseController
    {
        private readonly ILogger<ComentarioController> _logger;
        private readonly IComentarioRepository _comentarioRepository;

        public ComentarioController (ILogger<ComentarioController> logger, IComentarioRepository comentarioRepository, IUsuarioRepository usuarioRepository) : base (usuarioRepository)
        {
            _logger = logger;
            _comentarioRepository = comentarioRepository;
        }

        [HttpPut]
        public IActionResult Comentar([FromBody] ComentarioRequisicaoDto comentariodto)
        {
            try
            {
                if (comentariodto != null)
                {
                    if(String.IsNullOrEmpty(comentariodto.Descricao) || String.IsNullOrWhiteSpace(comentariodto.Descricao))
                    {
                        _logger.LogError("Comentário não pode ser vazio!");
                        return BadRequest("Comentário não pode ser vazio!");
                    }

                    Comentario comentario = new Comentario()
                    {
                        Descricao = comentariodto.Descricao,
                        IdPublicacao = comentariodto.IdPublicacao,
                        IdUsuario = LerToken().Id
                    };

                    _comentarioRepository.Comentar(comentario);
                }            

                return Ok("Comentário efetuado com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu um erro ao criar o comentário");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = $"Ocorreu o seguinte erro: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
