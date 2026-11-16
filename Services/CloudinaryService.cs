using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MusicDB.WebApi.Utils;

namespace MusicDB.WebApi.Services;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(CloudinarySettings settings)
    {
        var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string?> UploadImagemAsync(IFormFile? arquivo, string pasta = "musicdb")
    {
        if (arquivo is null || arquivo.Length == 0)
            return null;

        await using var stream = arquivo.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(arquivo.FileName, stream),
            Folder = pasta,
            Transformation = new Transformation().Width(800).Height(800).Crop("limit")
        };

        var resultado = await _cloudinary.UploadAsync(uploadParams);

        if (resultado.Error is not null)
            throw new InvalidOperationException($"Erro ao enviar imagem para o Cloudinary: {resultado.Error.Message}");

        return resultado.SecureUrl?.ToString();
    }
}