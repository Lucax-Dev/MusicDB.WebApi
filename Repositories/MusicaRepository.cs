using Microsoft.EntityFrameworkCore;
using MusicDB.WebApi.Context;
using MusicDB.WebApi.Interfaces;
using MusicDB.WebApi.Models;

namespace MusicDB.WebApi.Repositories;

public class MusicaRepository : IMusicaRepository
{
    private readonly MusicDbContext _context;

    public MusicaRepository(MusicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Musica>> ListarTodasAsync()
    {
        return await _context.Musica
            .Include(m => m.IdGenero)
            .OrderBy(m => m.Titulo)
            .ToListAsync();
    }

    public async Task<Musica?> BuscarPorIdAsync(int id)
    {
        return await _context.Musica
            .Include(m => m.IdGenero)
            .Include(m => m.IdUsuario)
            .FirstOrDefaultAsync(m => m.IdMusica == id);
    }

    public async Task<List<Musica>> BuscarPorGeneroAsync(int idGenero)
    {
        return await _context.Musica
            .Include(m => m.IdGenero)
            .Where(m => m.IdGenero.Any(g => g.IdGenero == idGenero))
            .OrderBy(m => m.Titulo)
            .ToListAsync();
    }

    public async Task<List<Musica>> BuscarPorTermoAsync(string termo)
    {
        var busca = termo.Trim().ToLower();

        return await _context.Musica
            .Include(m => m.IdGenero)
            .Where(m => m.Titulo.ToLower().Contains(busca)
                     || m.Artista.ToLower().Contains(busca)
                     || (m.Album != null && m.Album.ToLower().Contains(busca)))
            .OrderBy(m => m.Titulo)
            .ToListAsync();
    }

    public async Task<Musica> CadastrarAsync(Musica musica, List<int> idsGeneros)
    {
        var generos = await _context.Genero
            .Where(g => idsGeneros.Contains(g.IdGenero))
            .ToListAsync();

        if (generos.Count == 0)
            throw new ArgumentException("Nenhum gênero válido foi encontrado para associar à música.");

        musica.IdGenero = generos;

        _context.Musica.Add(musica);
        await _context.SaveChangesAsync();
        return musica;
    }

    public async Task<Musica> AtualizarAsync(Musica musica, List<int> idsGeneros)
    {
        var generos = await _context.Genero
            .Where(g => idsGeneros.Contains(g.IdGenero))
            .ToListAsync();

        if (generos.Count == 0)
            throw new ArgumentException("A música deve estar associada a pelo menos um gênero válido.");

        musica.IdGenero.Clear();
        foreach (var genero in generos)
        {
            musica.IdGenero.Add(genero);
        }

        await _context.SaveChangesAsync();
        return musica;
    }

    public async Task ExcluirAsync(Musica musica)
    {
        musica.IdGenero.Clear();
        musica.IdUsuario.Clear();

        _context.Musica.Remove(musica);
        await _context.SaveChangesAsync();
    }
}