using MusicDB.WebApi.Models;

namespace MusicDB.WebApi.Interfaces;

public interface IUsuarioRepository
{
    Task<List<Usuario>> ListarTodosAsync();
    Task<Usuario?> BuscarPorIdAsync(int id);
    Task<Usuario?> BuscarPorEmailAsync(string email);
    Task<Usuario?> BuscarPorEmailSenha(string email, string senha);
    Task<Usuario> CadastrarAsync(Usuario usuario);
    Task<Usuario> AtualizarAsync(Usuario usuario);
    Task ExcluirAsync(Usuario usuario);
    Task<bool> AdicionarFavoritoAsync(int idUsuario, int idMusica);
    Task<bool> RemoverFavoritoAsync(int idUsuario, int idMusica);
    Task<List<Musica>> ListarFavoritosAsync(int idUsuario);
}