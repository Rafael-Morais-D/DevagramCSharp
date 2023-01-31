using DevagramCSharp.Models;

namespace DevagramCSharp.Repository.Impl
{
    public class SeguidorRepositoryImpl : ISeguidorRepository
    {
        private readonly DevagramContext _context;

        public SeguidorRepositoryImpl (DevagramContext context)
        {
            _context = context;
        }

        public bool Seguir(Seguidor seguidor)
        {
            try
            {
                _context.Add(seguidor);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Desseguir(Seguidor seguidor)
        {
            try
            {
                _context.Remove(seguidor);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Seguidor GetSeguidor(int idSeguidor, int idSeguido)
        {
            return _context.Seguidores.FirstOrDefault(s => s.IdUsuarioSeguidor == idSeguidor && s.IdUsuarioSeguido == idSeguido);
        }

        public int GetQtdeSeguidores(int idUsuario)
        {
            return _context.Seguidores.Count(s => s.IdUsuarioSeguido == idUsuario);
        }

        public int GetQtdeSeguindo(int idUsuario)
        {
            return _context.Seguidores.Count(s => s.IdUsuarioSeguidor == idUsuario);
        }
    }
}
