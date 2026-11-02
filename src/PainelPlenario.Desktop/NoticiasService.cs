using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PainelPlenario.Desktop;

public class NoticiasService
{
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(8) };
    private const string UrlRssAgenciaBrasil = "https://agenciabrasil.ebc.com.br/rss/ultimasnoticias/feed.xml";

    public static async Task<List<string>> ObterNoticiasAsync()
    {
        try
        {
            // Tenta buscar o RSS da Agência Brasil
            var xmlString = await _httpClient.GetStringAsync(UrlRssAgenciaBrasil);
            var xdoc = XDocument.Parse(xmlString);

            // Pega os títulos das últimas 10 notícias
            var titulos = xdoc.Descendants("item")
                .Select(item => item.Element("title")?.Value?.Trim())
                .Where(titulo => !string.IsNullOrWhiteSpace(titulo))
                .Take(10)
                .Cast<string>()
                .ToList();

            if (titulos.Any())
            {
                return titulos;
            }
        }
        catch
        {
            // Em caso de falha de conexão (sem internet), cai no fallback abaixo silenciosamente
        }

        // Fallback institucional automático se estiver offline
        return new List<string>
        {
            "CÂMARA MUNICIPAL: Transparência e Cidadania em favor da nossa população",
            "Sessão Plenária em Andamento: Acompanhe os debates e as votações dos projetos de lei",
            "Portal da Transparência: Atas, pautas e documentos legislativos disponíveis a todos os cidadãos",
            "Mesa Diretora: Conduzindo os trabalhos legislativos com responsabilidade pública"
        };
    }
}