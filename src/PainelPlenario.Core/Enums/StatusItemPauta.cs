using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPlenario.Core.Enums
{
    public enum StatusItemPauta
    {
        Pendente = 1,      // Aguardando ser chamada
        EmDiscussao = 2,   // Vereadores na tribuna debatendo a matéria
        EmVotacao = 3,     // Votação aberta no painel
        Concluido = 4,     // Votação encerrada e resultado proclamado
        Retirado = 5       // Pedido de vista ou retirada pelo autor
    }
}
