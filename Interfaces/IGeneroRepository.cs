using MusicDB.WebApi.Models;

namespace MusicDB.WebApi.Interfaces;

public interface IGeneroRepository
{
    Task<List<Genero>> ListarTodosAsync();
    Task<Genero?> BuscarPorIdAsync(int id);
    Task<Genero?> BuscarPorNomeAsync(string nome);
    Task<Genero> CadastrarAsync(Genero genero);
    Task<Genero> AtualizarAsync(Genero genero);
    Task ExcluirAsync(Genero genero);
}