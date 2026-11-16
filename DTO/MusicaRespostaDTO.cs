using MusicDB.WebApi.Models;

namespace MusicDB.WebApi.DTO;

public class MusicaRespostaDTO
{
    public int IdMusica { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Artista { get; set; } = string.Empty;
    public string Letra { get; set; } = string.Empty;
    public string? Album { get; set; }
    public DateOnly DataLancamento { get; set; }
    public int Duracao { get; set; }
    public string? ImagemCapa { get; set; }
    public List<GeneroRespostaDTO> Generos { get; set; } = new();

    public static MusicaRespostaDTO Converter(Musica musica)
    {
        return new MusicaRespostaDTO
        {
            IdMusica = musica.IdMusica,
            Titulo = musica.Titulo,
            Artista = musica.Artista,
            Letra = musica.Letra,
            Album = musica.Album,
            DataLancamento = musica.DataLancamento,
            Duracao = musica.Duracao,
            ImagemCapa = musica.ImagemCapa,
            Generos = musica.IdGenero?
                .Select(GeneroRespostaDTO.Converter)
                .ToList() ?? new List<GeneroRespostaDTO>()
        };
    }
}