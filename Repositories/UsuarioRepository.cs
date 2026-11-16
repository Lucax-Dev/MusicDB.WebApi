using Microsoft.EntityFrameworkCore;
using MusicDB.WebApi.Context;
using MusicDB.WebApi.Interfaces;
using MusicDB.WebApi.Models;
using MusicDB.WebApi.Utils;

namespace MusicDB.WebApi.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly MusicDbContext _context;

    public UsuarioRepository(MusicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> ListarTodosAsync()
    {
        return await _context.Usuario
            .Include(u => u.IdMusica)
                .ThenInclude(m => m.IdGenero)
            .OrderBy(u => u.Nome)
            .ToListAsync();
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        return await _context.Usuario
            .Include(u => u.IdMusica)
                .ThenInclude(m => m.IdGenero)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);
    }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        return await _context.Usuario
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());
    }

    public async Task<Usuario?> BuscarPorEmailSenha(string email, string senha)
    {
        var usuario = await _context.Usuario
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());

        if (usuario is null)
            return null;

        if (!Criptografia.VerificarSenha(senha, usuario.Senha))
            return null;

        return usuario;
    }

    public async Task<Usuario> CadastrarAsync(Usuario usuario)
    {
        _context.Usuario.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario> AtualizarAsync(Usuario usuario)
    {
        _context.Usuario.Update(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task ExcluirAsync(Usuario usuario)
    {
        // Executa o DELETE no EF Core, que aciona a trigger TR_UsuarioSoftDelete (Ativo = 0)
        _context.Usuario.Remove(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AdicionarFavoritoAsync(int idUsuario, int idMusica)
    {
        var usuario = await _context.Usuario
            .Include(u => u.IdMusica)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

        var musica = await _context.Musica.FindAsync(idMusica);

        if (usuario is null || musica is null)
            return false;

        if (!usuario.IdMusica.Any(m => m.IdMusica == idMusica))
        {
            usuario.IdMusica.Add(musica);
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> RemoverFavoritoAsync(int idUsuario, int idMusica)
    {
        var usuario = await _context.Usuario
            .Include(u => u.IdMusica)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

        if (usuario is null)
            return false;

        var musicaFavorita = usuario.IdMusica.FirstOrDefault(m => m.IdMusica == idMusica);

        if (musicaFavorita is null)
            return false;

        usuario.IdMusica.Remove(musicaFavorita);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Musica>> ListarFavoritosAsync(int idUsuario)
    {
        var usuario = await _context.Usuario
            .Include(u => u.IdMusica)
                .ThenInclude(m => m.IdGenero)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

        return usuario?.IdMusica
            .OrderBy(m => m.Titulo)
            .ToList() ?? new List<Musica>();
    }
}