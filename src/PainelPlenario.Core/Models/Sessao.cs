using PainelPlenario.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPlenario.Core.Models
{
    public class Sessao
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Numero { get; set; } = 1;
        public int Ano { get; set; } = DateTime.Now.Year;
        public TipoSessao Tipo { get; set; } = TipoSessao.Ordinaria;
        public DateTime Data { get; set; } = DateTime.Today;
        // Horário em que o operador abriu os trabalhos
        public DateTime? HorarioInicio { get; set; }
        // Horário de encerramento
        public DateTime? HorarioFim { get; set; }
        // Status atual da sessão
        public StatusSessao Status { get; set; } = StatusSessao.NaoIniciada;
        public List<ItemPauta> ItensPauta { get; set; } = new();
        // Lista de presença dos vereadores nesta sessão
        public List<RegistroPresenca> Presencas { get; set; } = new();

        // Propriedade facilitadora: gera o título oficial automaticamente
        // Ex: "12ª Sessão Ordinária de 2026"
        public string TituloFormatado => $"{Numero}ª Sessão {Tipo} de {Ano}";
    }
}
