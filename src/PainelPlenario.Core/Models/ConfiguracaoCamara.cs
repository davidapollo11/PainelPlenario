using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPlenario.Core.Models
{
    public class ConfiguracaoCamara
    {
        public int Id { get; set; } = 1;

        public string NomeCamara { get; set; } = "Câmara Municipal";
        public string Slogan { get; set; } = "A Casa do Povo em defesa da cidadania!";
        public string Cidade { get; set; } = "Município";
        public string Estado { get; set; } = "UF";
        public string Legislatura { get; set; } = "2021-2024";
        public string? BrasaoCaminho { get; set; }

        // PRESETS DO TEMPO DE FALA
        public int TempoPequenoExpedienteMinutos { get; set; } = 5;
        public int TempoGrandeExpedienteMinutos { get; set; } = 10;
        public int TempoAparteMinutos { get; set; } = 1;
        public int TempoQuestaoOrdemMinutos { get; set; } = 3;

        // REGRAS REGIMENTAIS
        public int TotalCadeiras { get; set; } = 15;
    }
}
