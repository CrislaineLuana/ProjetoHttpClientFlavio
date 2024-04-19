using ProjetoHttpClient.Models;

namespace ProjetoHttpClient.Services
{
    public interface ISessaoInterface
    {

        UsuarioModel BuscarSessao();
        void CriarSessao(UsuarioModel usuario);
        void RemoverSessao();

    }
}
