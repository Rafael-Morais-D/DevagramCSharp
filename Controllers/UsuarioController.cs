using DevagramCSharp.Dtos;
using DevagramCSharp.Models;
using DevagramCSharp.Repository;
using DevagramCSharp.Services;
using DevagramCSharp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevagramCSharp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : BaseController
    {
        public readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger,
            IUsuarioRepository usuarioRepository) : base(usuarioRepository)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult ObterUsuario()
        {
            try
            {
                Usuario usuario = LerToken();

                return Ok(new UsuarioRespostaDto{
                    Nome = usuario.Nome,
                    Email = usuario.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu um erro ao obter o usuário");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = $"Ocorreu o seguinte erro: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SalvarUsuario([FromForm] UsuarioRequisicaoDto usuariodto)
        {
            try
            {
                if(usuariodto != null)
                {
                    var erros = new List<string>();

                    if(string.IsNullOrEmpty(usuariodto.Nome) || string.IsNullOrWhiteSpace(usuariodto.Nome))
                    {
                        erros.Add("Nome inválido");
                    }
                    if (string.IsNullOrEmpty(usuariodto.Email) || string.IsNullOrWhiteSpace(usuariodto.Email) || !usuariodto.Email.Contains("@"))
                    {
                        erros.Add("E-mail inválido");
                    }
                    if (string.IsNullOrEmpty(usuariodto.Senha) || string.IsNullOrWhiteSpace(usuariodto.Senha))
                    {
                        erros.Add("Senha inválida");
                    }

                    if(erros.Count > 0)
                    {
                        return BadRequest(new ErrorRespostaDto()
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Erros = erros
                        });
                    }

                    CosmicService cosmicService = new CosmicService();

                    Usuario usuario = new Usuario()
                    {
                        Nome = usuariodto.Nome,
                        Email = usuariodto.Email,
                        Senha = usuariodto.Senha,
                        FotoPerfil = cosmicService.EnviarImagem(new ImagemDto { Imagem = usuariodto.FotoPerfil, Nome = usuariodto.Nome.Replace(" ", "") })
                    };

                    usuario.Senha = MD5Utils.GerarHashMD5(usuario.Senha);
                    usuario.Email = usuario.Email.ToLower();

                    if (!_usuarioRepository.VerificarEmail(usuario.Email))
                    {
                        _usuarioRepository.Salvar(usuario);
                    }
                    else
                    {
                        return BadRequest(new ErrorRespostaDto()
                        {
                            Descricao = "Usuário já cadastrado no sistema com esse email!",
                            Status = StatusCodes.Status400BadRequest
                        });
                    }
                }

                return Ok("Usuário cadastrado com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError("Ocorreu um erro ao cadastrar o usuário");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorRespostaDto()
                {
                    Descricao = $"Ocorreu o seguinte erro: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
