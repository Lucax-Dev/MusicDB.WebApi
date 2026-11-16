using System.Security.Cryptography;
using System.Text;

namespace MusicDB.WebApi.Utils;

public static class Criptografia
{
    public static string GerarHash(string senha)
    {
        var bytes = Encoding.UTF8.GetBytes(senha);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    public static bool VerificarSenha(string senhaDigitada, string hashSalvo)
    {
        var hashDigitado = GerarHash(senhaDigitada);
        return string.Equals(hashDigitado, hashSalvo, StringComparison.OrdinalIgnoreCase);
    }
}