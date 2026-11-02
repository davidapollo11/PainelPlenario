using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPlenario.Core.Enums
{
    public enum ModoTelao
    {
        AguardandoInicio = 1,
        QuorumPresenca = 2,     // Lista de presença e contagem de quórum
        MateriaEmDiscussao = 3, // Texto e ementa do projeto sendo debatido
        TribunaCronometro = 4,  // Foto do orador + cronômetro regressivo
        VotacaoNominal = 5,
        ResultadoProclamado = 6 // Banner solene: "APROVADO" ou "REJEITADO"
    }
}
