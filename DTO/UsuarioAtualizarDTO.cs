using System.ComponentModel.DataAnnotations;

namespace MusicDB.WebApi.DTO;

public class UsuarioAtualizarDTO
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    [StringLength(50)]
    public string Email { get; set; } = string.Empty;

    public string? Senha { get; set; }

    public string? TipoUsuario { get; set; }

    public bool? Ativo { get; set; }

    public IFormFile? FotoPerfil { get; set; }
}