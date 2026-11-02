using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PainelPlenario.Core.Enums;
using PainelPlenario.Core.Models;
using PainelPlenario.Core.Services;

namespace PainelPlenario.Desktop;

public class RelatorioSessaoService
{
    static RelatorioSessaoService()
    {
        // Define a licença comunitária gratuita do QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static void GerarEAbrirPdf(PlenarioStateService estado)
    {
        if (estado.SessaoAtiva == null) return;

        var sessao = estado.SessaoAtiva;
        var vereadores = estado.Parlamentares;

        // Caminho do arquivo PDF gerado
        var nomeArquivo = $"Extrato_{sessao.TituloFormatado.Replace(" ", "_").Replace("ª", "")}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
        var caminhoCompleto = Path.Combine(Path.GetTempPath(), nomeArquivo);

        // Constrói o documento com código fluente do C#
        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

                // 1. CABEÇALHO DO DOCUMENTO
                page.Header().Column(col =>
                {
                    col.Item().AlignCenter().Text("REPÚBLICA FEDERATIVA DO BRASIL").Bold().FontSize(12);
                    col.Item().AlignCenter().Text("BAHIA / CÂMARA MUNICIPAL DE CAMPO FORMOSO").Bold().FontSize(14).FontColor(Colors.Blue.Darken3);
                    col.Item().AlignCenter().Text("MESA DIRETORA - SECRETARIA LEGISLATIVA").FontSize(9).FontColor(Colors.Grey.Medium);
                    col.Item().PaddingTop(8).LineHorizontal(1.5f).LineColor(Colors.Blue.Darken3);
                });

                // 2. CONTEÚDO PRINCIPAL
                page.Content().PaddingVertical(15).Column(col =>
                {
                    // TÍTULO DA SESSÃO
                    col.Item().AlignCenter().Text($"EXTRATO DA {sessao.TituloFormatado.ToUpper()}").Bold().FontSize(13);
                    col.Item().AlignCenter().Text($"Realizada em {sessao.Data:dd/MM/yyyy}").FontSize(10).Italic();

                    col.Item().PaddingTop(15).Text("1. REGISTRO OFICIAL DE QUÓRUM E PRESENÇA").Bold().FontSize(11).FontColor(Colors.Blue.Darken3);

                    // TABELA DE PRESENÇA
                    col.Item().PaddingTop(6).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Nome
                            columns.RelativeColumn(1); // Partido
                            columns.RelativeColumn(2); // Cargo na Mesa
                            columns.RelativeColumn(1.5f); // Status
                        });

                        // Cabeçalho da tabela
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Parlamentar").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Partido").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Cargo").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Presença").Bold();
                        });

                        foreach (var v in vereadores)
                        {
                            var presenca = sessao.Presencas.FirstOrDefault(p => p.ParlamentarId == v.Id);
                            var presente = presenca?.Presente ?? false;

                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.NomePolitico);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.Partido);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(v.CargoMesa);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(presente ? "PRESENTE" : "AUSENTE")
                                .Bold().FontColor(presente ? Colors.Green.Darken2 : Colors.Red.Darken2);
                        }
                    });

                    // 2. ORDEM DO DIA / VOTAÇÕES
                    col.Item().PaddingTop(20).Text("2. ESPELHO DAS VOTAÇÕES DA ORDEM DO DIA").Bold().FontSize(11).FontColor(Colors.Blue.Darken3);

                    if (!sessao.ItensPauta.Any())
                    {
                        col.Item().PaddingTop(6).Text("Nenhuma matéria cadastrada na pauta desta sessão.").Italic();
                    }
                    else
                    {
                        foreach (var item in sessao.ItensPauta.OrderBy(i => i.Ordem))
                        {
                            col.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(matCol =>
                            {
                                matCol.Item().Row(r =>
                                {
                                    r.RelativeItem().Text($"{item.Tipo} nº {item.Numero} (Ordem: #{item.Ordem})").Bold().FontSize(11);

                                    var textoRes = item.Status == StatusItemPauta.Concluido
                                        ? item.Resultado?.ToString().ToUpper()
                                        : "NÃO VOTADO";
                                    var corRes = item.Resultado == ResultadoVotacao.Aprovado ? Colors.Green.Darken2 : Colors.Red.Darken2;

                                    r.ConstantItem(100).AlignRight().Text(textoRes).Bold().FontColor(corRes);
                                });

                                matCol.Item().PaddingTop(4).Text($"Autor: {item.Autor}").FontSize(9).Italic();
                                matCol.Item().PaddingTop(4).Text($"Ementa: {item.Ementa}").FontSize(9);
                            });
                        }
                    }

                    // 3. CAMPOS PARA ASSINATURA DA MESA
                    col.Item().PaddingTop(40).Row(r =>
                    {
                        r.RelativeItem().PaddingRight(20).Column(c =>
                        {
                            c.Item().LineHorizontal(1);
                            c.Item().AlignCenter().Text("Presidente da Câmara Municipal").Bold().FontSize(9);
                        });

                        r.RelativeItem().PaddingLeft(20).Column(c =>
                        {
                            c.Item().LineHorizontal(1);
                            c.Item().AlignCenter().Text("1º Secretário da Mesa Diretora").Bold().FontSize(9);
                        });
                    });
                });

                // 3. RODAPÉ DO DOCUMENTO
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Documento oficial emitido eletronicamente pelo Sistema de Plenário em ");
                    x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).Bold();
                });
            });
        });

        // Gera o arquivo PDF no disco
        documento.GeneratePdf(caminhoCompleto);

        // Abre automaticamente o PDF no visualizador padrão do Windows (Edge / Chrome / Acrobat)
        Process.Start(new ProcessStartInfo
        {
            FileName = caminhoCompleto,
            UseShellExecute = true
        });
    }
}