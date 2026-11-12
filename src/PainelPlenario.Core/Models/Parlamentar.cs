namespace PainelPlenario.Core.Models
{
    public class Parlamentar
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NomeCompleto { get; set; } = string.Empty;
        // Nome como aparece no painel e na urna (ex: Zé da Farmácia)
        public string NomePolitico { get; set; } = string.Empty;
        public string Partido { get; set; } = string.Empty;
        public string CargoMesa { get; set; } = "Vereador";
        public string? FotoCaminho { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
