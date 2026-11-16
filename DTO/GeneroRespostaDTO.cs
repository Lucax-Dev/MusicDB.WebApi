using MusicDB.WebApi.Models;

namespace MusicDB.WebApi.DTO;

public class GeneroRespostaDTO
{
    public int IdGenero { get; set; }
    public string Nome { get; set; } = string.Empty;

    public static GeneroRespostaDTO Converter(Genero genero)
    {
        return new GeneroRespostaDTO
        {
            IdGenero = genero.IdGenero,
            Nome = genero.Nome
        };
    }
}