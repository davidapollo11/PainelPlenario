using PainelPlenario.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPlenario.Core.Models
{
    public class RegistroVoto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        // Qual matéria está sendo votada
        public Guid ItemPautaId { get; set; }
        // Qual vereador votou
        public Guid ParlamentarId { get; set; }
        // Qual foi o voto (Sim, Não, Abstenção, Ausente)
        public TipoVoto Voto { get; set; } = TipoVoto.Ausente;
        public DateTime DataHora { get; set; } = DateTime.Now;
    }
}
