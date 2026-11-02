using PainelPlenario.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPlenario.Core.Models
{
    public class ItemPauta
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SessaoId { get; set; }
        // Ordem no roteiro da sessão (1º, 2º, 3º...)
        public int Ordem { get; set; } = 1;
        // Tipo da matéria (ex: "Projeto de Lei", "Requerimento", "Indicação", "Moção")
        public string Tipo { get; set; } = "Projeto de Lei";
        // Número identificador (ex: "042/2026")
        public string Numero { get; set; } = string.Empty;
        // Quem propôs (ex: "Executivo Municipal", "Ver. Zé da Farmácia")
        public string Autor { get; set; } = string.Empty;
        // Resumo do projeto que é lido e projetado no telão
        public string Ementa { get; set; } = string.Empty;
        // Estado atual do item na sessão
        public StatusItemPauta Status { get; set; } = StatusItemPauta.Pendente;
        // Resultado final proclamado pelo Presidente (Aprovado / Rejeitado)
        public ResultadoVotacao? Resultado { get; set; }
    }
}
