using System.ComponentModel.DataAnnotations;

namespace MusicDB.WebApi.DTO;

public class MusicaDTO
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(100)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O artista é obrigatório.")]
    [StringLength(100)]
    public string Artista { get; set; } = string.Empty;

    [Required(ErrorMessage = "A letra é obrigatória.")]
    public string Letra { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Album { get; set; }

    [Required(ErrorMessage = "A data de lançamento é obrigatória.")]
    public DateOnly DataLancamento { get; set; }

    [Required(ErrorMessage = "A duração é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "A duração deve ser maior que zero.")]
    public int Duracao { get; set; }

    public IFormFile? ImagemCapa { get; set; }

    [Required(ErrorMessage = "Informe pelo menos um gênero.")]
    [MinLength(1, ErrorMessage = "A música deve estar associada a pelo menos um gênero.")]
    public List<int> IdsGeneros { get; set; } = new();
}