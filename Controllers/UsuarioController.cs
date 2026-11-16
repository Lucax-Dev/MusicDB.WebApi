using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicDB.WebApi.DTO;
using MusicDB.WebApi.Interfaces;
using MusicDB.WebApi.Models;
using MusicDB.WebApi.Services;
using MusicDB.WebApi.Utils;

namespace MusicDB.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // RN12
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly CloudinaryService _cloudinaryService;

    public UsuarioController(IUsuarioRepository usuarioRepository, CloudinaryService cloudinaryService)
    {
        _usuarioRepository = usuarioRepository;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")] // RN03
    public async Task<IActionResult> ListarTodos()
    {
        var usuarios = await _usuarioRepository.ListarTodosAsync();
        return Ok(usuarios.Select(UsuarioRespostaDTO.Converter));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var idLogado = ObterIdUsuarioLogado();
        var ehAdmin = User.IsInRole("Admin");

        if (!ehAdmin && idLogado != id)
            return Forbid();

        var usuario = await _usuarioRepository.BuscarPorIdAsync(id);
        if (usuario is null)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        return Ok(UsuarioRespostaDTO.Converter(usuario));
    }

    [HttpPost]
    // [Authorize(Roles = "Admin")] // RN03
    [AllowAnonymous]
    public async Task<IActionResult> Cadastrar([FromForm] UsuarioCriarDTO dto)
    {
        // RN01: E-mail único
        var emailExistente = await _usuarioRepository.BuscarPorEmailAsync(dto.Email);
        if (emailExistente is not null)
            return BadRequest(new { mensagem = "Já existe um usuário cadastrado com este e-mail." });

        // Se não for um Admin autenticado criando a conta, força o perfil comum
        if (!User.IsInRole("Admin"))
            dto.TipoUsuario = "Usuario";

        if (dto.TipoUsuario != "Admin" && dto.TipoUsuario != "Usuario")
            return BadRequest(new { mensagem = "TipoUsuario deve ser 'Admin' ou 'Usuario'." });

        var urlFoto = await _cloudinaryService.UploadImagemAsync(dto.FotoPerfil, "musicdb/usuarios");

        var novoUsuario = new Usuario
        {
            Nome = dto.Nome.Trim(),
            Email = dto.Email.Trim(),
            Senha = Criptografia.GerarHash(dto.Senha),
            TipoUsuario = dto.TipoUsuario,
            Ativo = true,
            FotoPerfil = urlFoto
        };

        var criado = await _usuarioRepository.CadastrarAsync(novoUsuario);
        return CreatedAtAction(nameof(BuscarPorId), new { id = criado.IdUsuario }, UsuarioRespostaDTO.Converter(criado));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromForm] UsuarioAtualizarDTO dto)
    {
        var idLogado = ObterIdUsuarioLogado();
        var ehAdmin = User.IsInRole("Admin");

        // RN09: Usuário comum poderá editar somente seu próprio perfil
        if (!ehAdmin && idLogado != id)
            return Forbid();

        var usuario = await _usuarioRepository.BuscarPorIdAsync(id);
        if (usuario is null)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        // RN01: Verifica duplicidade de e-mail
        var emailExistente = await _usuarioRepository.BuscarPorEmailAsync(dto.Email);
        if (emailExistente is not null && emailExistente.IdUsuario != id)
            return BadRequest(new { mensagem = "Este e-mail já está em uso por outro usuário." });

        usuario.Nome = dto.Nome.Trim();
        usuario.Email = dto.Email.Trim();

        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            usuario.Senha = Criptografia.GerarHash(dto.Senha);
        }

        if (dto.FotoPerfil is not null)
        {
            usuario.FotoPerfil = await _cloudinaryService.UploadImagemAsync(dto.FotoPerfil, "musicdb/usuarios");
        }

        // Apenas Admin pode alterar perfil (Admin/Usuario) e status Ativo (RN03 / RN04)
        if (ehAdmin)
        {
            if (!string.IsNullOrWhiteSpace(dto.TipoUsuario))
            {
                if (dto.TipoUsuario != "Admin" && dto.TipoUsuario != "Usuario")
                    return BadRequest(new { mensagem = "TipoUsuario deve ser 'Admin' ou 'Usuario'." });

                usuario.TipoUsuario = dto.TipoUsuario;
            }

            if (dto.Ativo.HasValue)
            {
                usuario.Ativo = dto.Ativo.Value;
            }
        }

        var atualizado = await _usuarioRepository.AtualizarAsync(usuario);
        return Ok(UsuarioRespostaDTO.Converter(atualizado));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")] // RN03
    public async Task<IActionResult> Excluir(int id)
    {
        var usuario = await _usuarioRepository.BuscarPorIdAsync(id);
        if (usuario is null)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        // Aciona a trigger TR_UsuarioSoftDelete no banco (Ativo = 0 - RN04)
        await _usuarioRepository.ExcluirAsync(usuario);
        return NoContent();
    }

    // --- FAVORITOS (RN13) ---

    [HttpGet("favoritos")]
    public async Task<IActionResult> ListarMeusFavoritos()
    {
        var idLogado = ObterIdUsuarioLogado();
        var favoritos = await _usuarioRepository.ListarFavoritosAsync(idLogado);
        return Ok(favoritos.Select(MusicaRespostaDTO.Converter));
    }

    [HttpPost("favoritos/{idMusica:int}")]
    public async Task<IActionResult> AdicionarFavorito(int idMusica)
    {
        var idLogado = ObterIdUsuarioLogado();
        var sucesso = await _usuarioRepository.AdicionarFavoritoAsync(idLogado, idMusica);

        if (!sucesso)
            return NotFound(new { mensagem = "Usuário ou música não encontrados." });

        return Ok(new { mensagem = "Música adicionada aos favoritos." });
    }

    [HttpDelete("favoritos/{idMusica:int}")]
    public async Task<IActionResult> RemoverFavorito(int idMusica)
    {
        var idLogado = ObterIdUsuarioLogado();
        var sucesso = await _usuarioRepository.RemoverFavoritoAsync(idLogado, idMusica);

        if (!sucesso)
            return NotFound(new { mensagem = "Música não encontrada na sua lista de favoritos." });

        return NoContent();
    }

    private int ObterIdUsuarioLogado()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : 0;
    }
}