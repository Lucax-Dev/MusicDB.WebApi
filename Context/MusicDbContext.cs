using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MusicDB.WebApi.Models;

namespace MusicDB.WebApi.Context;

public partial class MusicDbContext : DbContext
{
    public MusicDbContext()
    {
    }

    public MusicDbContext(DbContextOptions<MusicDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Genero> Genero { get; set; }

    public virtual DbSet<Musica> Musica { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //----//
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genero>(entity =>
        {
            entity.HasKey(e => e.IdGenero).HasName("PK__Genero__0F8349882EFB1842");

            entity.HasMany(d => d.IdMusica).WithMany(p => p.IdGenero)
                .UsingEntity<Dictionary<string, object>>(
                    "MusicaGenero",
                    r => r.HasOne<Musica>().WithMany()
                        .HasForeignKey("IdMusica")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MusicaGenero_Musica"),
                    l => l.HasOne<Genero>().WithMany()
                        .HasForeignKey("IdGenero")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MusicaGenero_Genero"),
                    j =>
                    {
                        j.HasKey("IdGenero", "IdMusica");
                    });
        });

        modelBuilder.Entity<Musica>(entity =>
        {
            entity.HasKey(e => e.IdMusica).HasName("PK__Musica__F026ACA70DBFF746");

            entity.HasMany(d => d.IdUsuario).WithMany(p => p.IdMusica)
                .UsingEntity<Dictionary<string, object>>(
                    "Favorito",
                    r => r.HasOne<Usuario>().WithMany()
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Favorito_Usuario"),
                    l => l.HasOne<Musica>().WithMany()
                        .HasForeignKey("IdMusica")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Favorito_Musica"),
                    j =>
                    {
                        j.HasKey("IdMusica", "IdUsuario");
                    });
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF97176E8F7D");

            entity.ToTable("Usuario", tb => tb.HasTrigger("TR_UsuarioSoftDelete"));

            entity.Property(e => e.Ativo).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
