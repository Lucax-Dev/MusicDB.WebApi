using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicDB.WebApi.DTO;
using MusicDB.WebApi.Interfaces;
using MusicDB.WebApi.Models;

namespace MusicDB.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // RN12
public class GeneroController : ControllerBase
{
    private readonly IGeneroRepository _generoRepository;

    public GeneroController(IGeneroRepository generoRepository)
    {
        _generoRepository = generoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> ListarTodos()
    {
        var generos = await _generoRepository.ListarTodosAsync();
        return Ok(generos.Select(GeneroRespostaDTO.Converter));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var genero = await _generoRepository.BuscarPorIdAsync(id);

        if (genero is null)
            return NotFound(new { mensagem = "Gênero não encontrado." });

        return Ok(GeneroRespostaDTO.Converter(genero));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Cadastrar([FromBody] GeneroDTO dto)
    {
        // RN11: Gêneros não poderão ser cadastrados com nomes duplicados
        var existente = await _generoRepository.BuscarPorNomeAsync(dto.Nome);
        if (existente is not null)
            return BadRequest(new { mensagem = "Já existe um gênero cadastrado com esse nome." });

        var novoGenero = new Genero
        {
            Nome = dto.Nome.Trim()
        };

        var criado = await _generoRepository.CadastrarAsync(novoGenero);
        return CreatedAtAction(nameof(BuscarPorId), new { id = criado.IdGenero }, GeneroRespostaDTO.Converter(criado));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] GeneroDTO dto)
    {
        var genero = await _generoRepository.BuscarPorIdAsync(id);
        if (genero is null)
            return NotFound(new { mensagem = "Gênero não encontrado." });

        var duplicado = await _generoRepository.BuscarPorNomeAsync(dto.Nome);
        if (duplicado is not null && duplicado.IdGenero != id)
            return BadRequest(new { mensagem = "Já existe outro gênero cadastrado com esse nome." });

        genero.Nome = dto.Nome.Trim();
        var atualizado = await _generoRepository.AtualizarAsync(genero);

        return Ok(GeneroRespostaDTO.Converter(atualizado));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(int id)
    {
        var genero = await _generoRepository.BuscarPorIdAsync(id);
        if (genero is null)
            return NotFound(new { mensagem = "Gênero não encontrado." });

        if (genero.IdMusica.Any())
            return BadRequest(new { mensagem = "Não é possível excluir um gênero que possui músicas vinculadas." });

        await _generoRepository.ExcluirAsync(genero);
        return NoContent();
    }
}