using Newtonsoft.Json;
using ProjetoHttpClient.Models;

namespace ProjetoHttpClient.Services
{
    public class SessaoService : ISessaoInterface
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        public SessaoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UsuarioModel BuscarSessao()
        {
            string sessaoUsuario = _httpContextAccessor.HttpContext.Session.GetString("SessaoUsuario");

            if(string.IsNullOrEmpty(sessaoUsuario))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<UsuarioModel>(sessaoUsuario);
        }

        public void CriarSessao(UsuarioModel usuario)
        {
            string usuarioJson = JsonConvert.SerializeObject(usuario);
            _httpContextAccessor.HttpContext.Session.SetString("SessaoUsuario", usuarioJson);
        }

        public void RemoverSessao()
        {
            _httpContextAccessor.HttpContext.Session.Remove("SessaoUsuario");
        }
    }
}
