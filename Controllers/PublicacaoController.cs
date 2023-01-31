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
        private readonly IComentarioRepository _comentarioRepository;
        private readonly ICurtidaRepository _curtidaRepository;

        public PublicacaoController(ILogger<PublicacaoController> logger, IPublicacaoRepository publicacaoRepository, IUsuarioRepository usuarioRepository, IComentarioRepository comentarioRepository, ICurtidaRepository curtidaRepository) : base(usuarioRepository)
        {
            _logger = logger;
            _publicacaoRepository = publicacaoRepository;
            _comentarioRepository = comentarioRepository;
            _curtidaRepository = curtidaRepository;
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

        [HttpGet]
        [Route("feed")]
        public IActionResult FeedHome()
        {
            try
            {
                List<PublicacaoFeedRespostaDto> feed = _publicacaoRepository.GetPublicacoesFeed(LerToken().Id);

                if (feed != null)
                {
                    foreach (PublicacaoFeedRespostaDto feedResposta in feed)
                    {
                        Usuario usuario = _usuarioRepository.GetUsuarioPorId(feedResposta.IdUsuario);
                        UsuarioRespostaDto usuarioRespostaDto = new UsuarioRespostaDto()
                        {
                            Nome = usuario.Nome,
                            Avatar = usuario.FotoPerfil,
                            IdUsuario = usuario.Id
                        };
                        feedResposta.Usuario = usuarioRespostaDto;

                        List<Comentario> comentarios = _comentarioRepository.GetComentarioPorPublicacao(feedResposta.IdPublicacao);
                        feedResposta.Comentarios = comentarios;

                        List<Curtida> curtidas = _curtidaRepository.GetCurtidaPorPublicacao(feedResposta.IdPublicacao);
                        feedResposta.Curtidas = curtidas;
                    }

                    return Ok(feed);
                }

                throw new Exception("O Feed da Home está vazio");                
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu um erro ao carregar o feed da Home");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = $"Ocorreu o seguinte erro: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpGet]
        [Route("feedusuario")]
        public IActionResult FeedUsuario(int idUsuario)
        {
            try
            {
                List<PublicacaoFeedRespostaDto> feed = _publicacaoRepository.GetPublicacoesFeedUsuario(idUsuario);

                if (feed != null)
                {
                    foreach (PublicacaoFeedRespostaDto feedResposta in feed)
                    {
                        Usuario usuario = _usuarioRepository.GetUsuarioPorId(feedResposta.IdUsuario);
                        UsuarioRespostaDto usuarioRespostaDto = new UsuarioRespostaDto()
                        {
                            Nome = usuario.Nome,
                            Avatar = usuario.FotoPerfil,
                            IdUsuario = usuario.Id
                        };
                        feedResposta.Usuario = usuarioRespostaDto;

                        List<Comentario> comentarios = _comentarioRepository.GetComentarioPorPublicacao(feedResposta.IdPublicacao);
                        feedResposta.Comentarios = comentarios;

                        List<Curtida> curtidas = _curtidaRepository.GetCurtidaPorPublicacao(feedResposta.IdPublicacao);
                        feedResposta.Curtidas = curtidas;
                    }

                    return Ok(feed);
                }

                throw new Exception("O Feed do Usuário está vazio");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu um erro ao carregar o feed do Usuário");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = $"Ocorreu o seguinte erro: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
