using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicDB.WebApi.DTO;
using MusicDB.WebApi.Interfaces;
using MusicDB.WebApi.Models;
using MusicDB.WebApi.Services;

namespace MusicDB.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // RN12
public class MusicaController : ControllerBase
{
    private readonly IMusicaRepository _musicaRepository;
    private readonly CloudinaryService _cloudinaryService;

    public MusicaController(IMusicaRepository musicaRepository, CloudinaryService cloudinaryService)
    {
        _musicaRepository = musicaRepository;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    public async Task<IActionResult> ListarTodas()
    {
        var musicas = await _musicaRepository.ListarTodasAsync();
        return Ok(musicas.Select(MusicaRespostaDTO.Converter));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var musica = await _musicaRepository.BuscarPorIdAsync(id);
        if (musica is null)
            return NotFound(new { mensagem = "Música não encontrada." });

        return Ok(MusicaRespostaDTO.Converter(musica));
    }

    [HttpGet("genero/{idGenero:int}")]
    public async Task<IActionResult> BuscarPorGenero(int idGenero)
    {
        var musicas = await _musicaRepository.BuscarPorGeneroAsync(idGenero);
        return Ok(musicas.Select(MusicaRespostaDTO.Converter));
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> BuscarPorTermo([FromQuery] string termo)
    {
        if (string.IsNullOrWhiteSpace(termo))
            return BadRequest(new { mensagem = "Informe um termo para busca." });

        var musicas = await _musicaRepository.BuscarPorTermoAsync(termo);
        return Ok(musicas.Select(MusicaRespostaDTO.Converter));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")] // RN02
    public async Task<IActionResult> Cadastrar([FromForm] MusicaDTO dto)
    {
        // RN07 e RN08
        var erroValidacao = ValidarRegrasMusica(dto);
        if (erroValidacao is not null)
            return BadRequest(new { mensagem = erroValidacao });

        var urlCapa = await _cloudinaryService.UploadImagemAsync(dto.ImagemCapa, "musicdb/capas");

        var novaMusica = new Musica
        {
            Titulo = dto.Titulo.Trim(),
            Artista = dto.Artista.Trim(),
            Letra = dto.Letra,
            Album = string.IsNullOrWhiteSpace(dto.Album) ? null : dto.Album.Trim(),
            DataLancamento = dto.DataLancamento,
            Duracao = dto.Duracao,
            ImagemCapa = urlCapa
        };

        try
        {
            var criada = await _musicaRepository.CadastrarAsync(novaMusica, dto.IdsGeneros);
            return CreatedAtAction(nameof(BuscarPorId), new { id = criada.IdMusica }, MusicaRespostaDTO.Converter(criada));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")] // RN02
    public async Task<IActionResult> Atualizar(int id, [FromForm] MusicaDTO dto)
    {
        var musica = await _musicaRepository.BuscarPorIdAsync(id);
        if (musica is null)
            return NotFound(new { mensagem = "Música não encontrada." });

        var erroValidacao = ValidarRegrasMusica(dto);
        if (erroValidacao is not null)
            return BadRequest(new { mensagem = erroValidacao });

        if (dto.ImagemCapa is not null)
        {
            musica.ImagemCapa = await _cloudinaryService.UploadImagemAsync(dto.ImagemCapa, "musicdb/capas");
        }

        musica.Titulo = dto.Titulo.Trim();
        musica.Artista = dto.Artista.Trim();
        musica.Letra = dto.Letra;
        musica.Album = string.IsNullOrWhiteSpace(dto.Album) ? null : dto.Album.Trim();
        musica.DataLancamento = dto.DataLancamento;
        musica.Duracao = dto.Duracao;

        try
        {
            var atualizada = await _musicaRepository.AtualizarAsync(musica, dto.IdsGeneros);
            return Ok(MusicaRespostaDTO.Converter(atualizada));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")] // RN02
    public async Task<IActionResult> Excluir(int id)
    {
        var musica = await _musicaRepository.BuscarPorIdAsync(id);
        if (musica is null)
            return NotFound(new { mensagem = "Música não encontrada." });

        await _musicaRepository.ExcluirAsync(musica);
        return NoContent();
    }

    private static string? ValidarRegrasMusica(MusicaDTO dto)
    {
        // RN05: Pelo menos um gênero
        if (dto.IdsGeneros is null || dto.IdsGeneros.Count == 0)
            return "A música deve estar associada a pelo menos um gênero.";

        // RN07: Duração maior que zero
        if (dto.Duracao <= 0)
            return "A duração da música deve ser maior que zero.";

        // RN08: Ano de lançamento entre 1900 e o ano atual
        var anoAtual = DateTime.Now.Year;
        if (dto.DataLancamento.Year < 1900 || dto.DataLancamento.Year > anoAtual)
            return $"O ano de lançamento deve estar entre 1900 e {anoAtual}.";

        return null;
    }
}