using System.ComponentModel.DataAnnotations;

namespace MusicDB.WebApi.DTO;

public class GeneroDTO
{
    [Required(ErrorMessage = "O nome do gênero é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome do gênero deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;
}