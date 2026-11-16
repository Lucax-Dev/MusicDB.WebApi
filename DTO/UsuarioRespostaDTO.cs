using MusicDB.WebApi.Models;

namespace MusicDB.WebApi.DTO;

public class UsuarioRespostaDTO
{
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TipoUsuario { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public string? FotoPerfil { get; set; }
    public List<MusicaRespostaDTO> Favoritos { get; set; } = new();

    public static UsuarioRespostaDTO Converter(Usuario usuario)
    {
        return new UsuarioRespostaDTO
        {
            IdUsuario = usuario.IdUsuario,
            Nome = usuario.Nome,
            Email = usuario.Email,
            TipoUsuario = usuario.TipoUsuario,
            Ativo = usuario.Ativo,
            FotoPerfil = usuario.FotoPerfil,
            Favoritos = usuario.IdMusica?
                .Select(MusicaRespostaDTO.Converter)
                .ToList() ?? new List<MusicaRespostaDTO>()
        };
    }
}