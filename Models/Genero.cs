using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MusicDB.WebApi.Models;

[Index("Nome", Name = "UQ_Genero_Nome", IsUnique = true)]
public partial class Genero
{
    [Key]
    public int IdGenero { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Nome { get; set; } = null!;

    [ForeignKey("IdGenero")]
    [InverseProperty("IdGenero")]
    public virtual ICollection<Musica> IdMusica { get; set; } = new List<Musica>();
}
