using System.ComponentModel.DataAnnotations;

namespace MusicDB.WebApi.DTO;

public class UsuarioCriarDTO
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
    [StringLength(50)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;

    public string TipoUsuario { get; set; } = "Usuario";

    public IFormFile? FotoPerfil { get; set; }
}