using Microsoft.EntityFrameworkCore;
using MusicDB.WebApi.Context;
using MusicDB.WebApi.Interfaces;
using MusicDB.WebApi.Models;

namespace MusicDB.WebApi.Repositories;

public class GeneroRepository : IGeneroRepository
{
    private readonly MusicDbContext _context;

    public GeneroRepository(MusicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Genero>> ListarTodosAsync()
    {
        return await _context.Genero
            .OrderBy(g => g.Nome)
            .ToListAsync();
    }

    public async Task<Genero?> BuscarPorIdAsync(int id)
    {
        return await _context.Genero
            .Include(g => g.IdMusica)
            .FirstOrDefaultAsync(g => g.IdGenero == id);
    }

    public async Task<Genero?> BuscarPorNomeAsync(string nome)
    {
        return await _context.Genero
            .FirstOrDefaultAsync(g => g.Nome.ToLower() == nome.Trim().ToLower());
    }

    public async Task<Genero> CadastrarAsync(Genero genero)
    {
        _context.Genero.Add(genero);
        await _context.SaveChangesAsync();
        return genero;
    }

    public async Task<Genero> AtualizarAsync(Genero genero)
    {
        _context.Genero.Update(genero);
        await _context.SaveChangesAsync();
        return genero;
    }

    public async Task ExcluirAsync(Genero genero)
    {
        _context.Genero.Remove(genero);
        await _context.SaveChangesAsync();
    }
}