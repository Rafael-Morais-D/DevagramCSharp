using DevagramCSharp.Models;

namespace DevagramCSharp.Repository
{
    public interface IUsuarioRepository
    {
        Usuario GetUsuarioPorId(int id);
        Usuario GetUsuarioPorLoginSenha(string email, string senha);
        public void Salvar(Usuario usuario);

        public void AtualizarUsuario(Usuario usuario);

        public bool VerificarEmail(string email);
    }
}
