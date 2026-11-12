using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Concurrent;
using System.IO;

namespace PainelPlenario.Desktop.Helpers;

public static class FotoHelper
{
    // Pasta fixa no AppData Local (não se perde ao atualizar ou rodar dotnet watch)
    private static readonly string PastaFotos = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PainelPlenario", "Fotos", "Parlamentares");

    // Cache em memória para o relógio de 1s do Telão não ficar relendo o disco
    private static readonly ConcurrentDictionary<string, string> _cacheDataUrls = new();

    public static async Task<string> SalvarFotoAsync(IBrowserFile arquivo)
    {
        Directory.CreateDirectory(PastaFotos);

        var ehPng = arquivo.ContentType.Contains("png", StringComparison.OrdinalIgnoreCase)
                    || arquivo.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase);

        var formato = ehPng ? "image/png" : "image/jpeg";
        var extensao = ehPng ? ".png" : ".jpg";

        // Redimensiona no navegador antes de gravar (máx 400x400px)
        var imagemRedimensionada = await arquivo.RequestImageFileAsync(formato, 400, 400);

        var nomeArquivo = $"{Guid.NewGuid():N}{extensao}";
        var caminhoCompleto = Path.Combine(PastaFotos, nomeArquivo);

        await using var origem = imagemRedimensionada.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
        await using var destino = File.Create(caminhoCompleto);
        await origem.CopyToAsync(destino);

        return nomeArquivo;
    }

    public static string? ObterDataUrl(string? nomeArquivo)
    {
        if (string.IsNullOrWhiteSpace(nomeArquivo))
            return null;

        if (_cacheDataUrls.TryGetValue(nomeArquivo, out var cached))
            return cached;

        var caminhoCompleto = Path.Combine(PastaFotos, nomeArquivo);
        if (!File.Exists(caminhoCompleto))
            return null;

        var ext = Path.GetExtension(caminhoCompleto).ToLowerInvariant();
        var mime = ext switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };

        var bytes = File.ReadAllBytes(caminhoCompleto);
        var dataUrl = $"data:{mime};base64,{Convert.ToBase64String(bytes)}";

        _cacheDataUrls[nomeArquivo] = dataUrl;
        return dataUrl;
    }

    public static void ExcluirArquivo(string? nomeArquivo)
    {
        if (string.IsNullOrWhiteSpace(nomeArquivo))
            return;

        _cacheDataUrls.TryRemove(nomeArquivo, out _);

        var caminhoCompleto = Path.Combine(PastaFotos, nomeArquivo);
        if (File.Exists(caminhoCompleto))
        {
            try { File.Delete(caminhoCompleto); } catch { }
        }
    }
}