using MusicDB.WebApi.Models;

namespace MusicDB.WebApi.Interfaces;

public interface IMusicaRepository
{
    Task<List<Musica>> ListarTodasAsync();
    Task<Musica?> BuscarPorIdAsync(int id);
    Task<List<Musica>> BuscarPorGeneroAsync(int idGenero);
    Task<List<Musica>> BuscarPorTermoAsync(string termo);
    Task<Musica> CadastrarAsync(Musica musica, List<int> idsGeneros);
    Task<Musica> AtualizarAsync(Musica musica, List<int> idsGeneros);
    Task ExcluirAsync(Musica musica);
}