using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPlenario.Core.Models
{
    public class RegistroPresenca
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SessaoId { get; set; }
        public Guid ParlamentarId { get; set; }
        // Se o vereador respondeu à chamada e está no plenário
        public bool Presente { get; set; } = false;
        // Se a ausência foi justificada oficialmente por ofício/atestado
        public bool Justificada { get; set; } = false;
        // Texto opcional da justificativa (ex: "Licença médica", "Missão oficial")
        public string? MotivoJustificativa { get; set; }
    }
}
