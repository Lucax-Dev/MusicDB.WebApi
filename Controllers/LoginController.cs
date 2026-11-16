using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MusicDB.WebApi.DTO;
using MusicDB.WebApi.Interfaces;
using MusicDB.WebApi.Utils;

namespace MusicDB.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly JwtSettings _jwtSettings;

    public LoginController(IUsuarioRepository usuarioRepository, JwtSettings jwtSettings)
    {
        _usuarioRepository = usuarioRepository;
        _jwtSettings = jwtSettings;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var usuario = await _usuarioRepository.BuscarPorEmailSenha(loginDto.Email, loginDto.Senha);

        if (usuario is null)
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

        // RN04: Usuários desativados não poderão acessar o sistema
        if (!usuario.Ativo)
            return Unauthorized(new { mensagem = "Usuário desativado. Acesso não permitido." });

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.TipoUsuario)
        };

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credenciais
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            usuario = new
            {
                usuario.IdUsuario,
                usuario.Nome,
                usuario.Email,
                usuario.TipoUsuario,
                usuario.FotoPerfil
            }
        });
    }
}