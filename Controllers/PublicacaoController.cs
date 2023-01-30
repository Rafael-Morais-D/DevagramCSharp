using DevagramCSharp.Dtos;
using DevagramCSharp.Models;
using DevagramCSharp.Repository;
using DevagramCSharp.Services;
using DevagramCSharp.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DevagramCSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicacaoController : BaseController
    {
        private readonly ILogger<PublicacaoController> _logger;
        private readonly IPublicacaoRepository _publicacaoRepository;

        public PublicacaoController(ILogger<PublicacaoController> logger, IPublicacaoRepository publicacaoRepository, IUsuarioRepository usuarioRepository) : base(usuarioRepository)
        {
            _logger = logger;
            _publicacaoRepository = publicacaoRepository;
        }

        [HttpPost]
        public IActionResult Publicar([FromForm] PublicacaoRequisicaoDto publicacaodto)
        {
            try
            {
                CosmicService cosmicService = new CosmicService();

                if(publicacaodto != null)
                {
                    if (String.IsNullOrEmpty(publicacaodto.Descricao) || String.IsNullOrWhiteSpace(publicacaodto.Descricao))
                    {
                        _logger.LogError("Descrição da publicação inválida!");
                        return BadRequest("Você deve colocar uma descrição na publicação!");
                    }

                    if (publicacaodto.Foto == null)
                    {
                        _logger.LogError("Foto não encontrada!");
                        return BadRequest("Você deve colocar uma foto na publicação!");
                    }

                    Publicacao publicacao = new Publicacao()
                    {
                        Descricao = publicacaodto.Descricao,
                        IdUsuario = LerToken().Id,
                        Foto = cosmicService.EnviarImagem(new ImagemDto { Imagem = publicacaodto.Foto, Nome = publicacaodto.Descricao })
                    };

                    _publicacaoRepository.Publicar(publicacao);
                }
                
                return Ok("Publicação efetuada com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu um erro ao criar a publicação");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = $"Ocorreu o seguinte erro: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
